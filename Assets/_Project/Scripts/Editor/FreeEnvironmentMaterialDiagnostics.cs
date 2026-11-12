#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Read-only diagnostic for imported environment materials.
    /// Does not modify source models, materials, prefabs, or scenes.
    /// </summary>
    public static class FreeEnvironmentMaterialDiagnostics
    {
        private const string ModelRoot = "Assets/_Project/Art/Environment/Runtime/Models";
        private const int SamplesPerGroup = 10;

        [MenuItem("Tools/Mia & Max/Assets/Diagnose Environment Materials")]
        public static void Run()
        {
            AssetDatabase.Refresh();
            string[] modelGuids = AssetDatabase.FindAssets("t:Model", new[] { ModelRoot });
            string[] materialGuids = AssetDatabase.FindAssets("t:Material", new[] { ModelRoot });
            string[] textureGuids = AssetDatabase.FindAssets("t:Texture", new[] { ModelRoot });

            var materials = new Dictionary<string, Material>(StringComparer.OrdinalIgnoreCase);
            int modelsScanned = 0, renderersScanned = 0, meshesWithVertexColors = 0;
            int meshesWithoutVertexColors = 0, rendererSlots = 0, emptySlots = 0;

            foreach (string guid in modelGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                foreach (UnityEngine.Object asset in AssetDatabase.LoadAllAssetsAtPath(path))
                {
                    Material embedded = asset as Material;
                    if (embedded != null)
                        materials[AssetDatabase.GetAssetPath(embedded) + "::" + embedded.name] = embedded;

                    Mesh mesh = asset as Mesh;
                    if (mesh != null)
                    {
                        if (mesh.colors != null && mesh.colors.Length == mesh.vertexCount) meshesWithVertexColors++;
                        else meshesWithoutVertexColors++;
                    }
                }

                GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (model == null) continue;
                modelsScanned++;
                foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>(true))
                {
                    renderersScanned++;
                    foreach (Material material in renderer.sharedMaterials)
                    {
                        rendererSlots++;
                        if (material == null) { emptySlots++; continue; }
                        materials[AssetDatabase.GetAssetPath(material) + "::" + material.name] = material;
                    }
                }
            }

            int materialsWithTextures = 0, materialsWithBaseMap = 0, materialsWithNonWhiteColor = 0;
            var shaderCounts = new SortedDictionary<string, int>();
            var texturedSamples = new List<string>();
            var coloredNoTextureSamples = new List<string>();
            var whiteNoTextureSamples = new List<string>();
            var standardSamples = new List<string>();

            foreach (Material material in materials.Values)
            {
                string shaderName = material.shader != null ? material.shader.name : "<missing shader>";
                if (!shaderCounts.ContainsKey(shaderName)) shaderCounts[shaderName] = 0;
                shaderCounts[shaderName]++;

                bool hasTexture = false, hasBaseMap = false;
                var textureDetails = new List<string>();
                try
                {
                    foreach (string propertyName in material.GetTexturePropertyNames())
                    {
                        Texture texture = material.GetTexture(propertyName);
                        if (texture == null) continue;
                        hasTexture = true;
                        if (propertyName == "_BaseMap" || propertyName == "_MainTex" || propertyName == "baseColorTexture")
                            hasBaseMap = true;
                        string texturePath = AssetDatabase.GetAssetPath(texture);
                        textureDetails.Add(propertyName + "=" + texture.name + " [" + texturePath + "]");
                    }
                }
                catch (Exception) { }

                if (hasTexture) materialsWithTextures++;
                if (hasBaseMap) materialsWithBaseMap++;

                Color color = material.HasProperty("_BaseColor") ? material.GetColor("_BaseColor") :
                    material.HasProperty("_Color") ? material.GetColor("_Color") : Color.white;
                bool nonWhite = Mathf.Abs(color.r - 1f) > 0.001f || Mathf.Abs(color.g - 1f) > 0.001f ||
                    Mathf.Abs(color.b - 1f) > 0.001f || Mathf.Abs(color.a - 1f) > 0.001f;
                if (nonWhite) materialsWithNonWhiteColor++;

                string line = "- " + material.name + " | asset: " + AssetDatabase.GetAssetPath(material) +
                    " | shader: " + shaderName + " | color: " + ColorUtility.ToHtmlStringRGBA(color) +
                    " | textures: " + (hasTexture ? string.Join(", ", textureDetails) : "none");

                AddSample(hasTexture ? texturedSamples : (nonWhite ? coloredNoTextureSamples : whiteNoTextureSamples), line);
                if (shaderName == "Standard") AddSample(standardSamples, line);
            }

            var report = new StringBuilder();
            report.AppendLine("MIA & MAX — ENVIRONMENT MATERIAL DIAGNOSTICS (READ ONLY)");
            report.AppendLine("Models found: " + modelGuids.Length + " | model prefabs scanned: " + modelsScanned);
            report.AppendLine("Standalone material assets: " + materialGuids.Length);
            report.AppendLine("Texture assets: " + textureGuids.Length);
            report.AppendLine("Unique embedded/assigned materials: " + materials.Count);
            report.AppendLine("Renderers scanned: " + renderersScanned + " | material slots: " + rendererSlots + " | empty slots: " + emptySlots);
            report.AppendLine("Materials with any assigned texture: " + materialsWithTextures);
            report.AppendLine("Materials with base/albedo texture property: " + materialsWithBaseMap);
            report.AppendLine("Materials with non-white color: " + materialsWithNonWhiteColor);
            report.AppendLine("Meshes with vertex colors: " + meshesWithVertexColors + " | without: " + meshesWithoutVertexColors);
            report.AppendLine();
            report.AppendLine("SHADERS:");
            foreach (var pair in shaderCounts) report.AppendLine("- " + pair.Key + ": " + pair.Value);
            AppendSamples(report, "TEXTURED MATERIALS (WITH TEXTURE ASSET PATHS):", texturedSamples);
            AppendSamples(report, "COLORED MATERIALS WITHOUT TEXTURES:", coloredNoTextureSamples);
            AppendSamples(report, "WHITE MATERIALS WITHOUT TEXTURES:", whiteNoTextureSamples);
            AppendSamples(report, "STANDARD SHADER MATERIALS:", standardSamples);
            report.AppendLine();
            report.AppendLine("Read-only diagnostic. No assets were changed.");

            Debug.Log(report.ToString());
            EditorUtility.DisplayDialog("Mia & Max — Material Diagnostics",
                "Models: " + modelGuids.Length + "\nUnique materials: " + materials.Count +
                "\nTextured materials: " + materialsWithTextures + "\nTexture assets: " + textureGuids.Length +
                "\nColored untextured materials: " + coloredNoTextureSamples.Count +
                "\n\nFull categorized report is in Console. No assets were changed.", "OK");
        }

        private static void AddSample(List<string> samples, string line)
        {
            if (samples.Count < SamplesPerGroup) samples.Add(line);
        }

        private static void AppendSamples(StringBuilder report, string title, List<string> samples)
        {
            report.AppendLine();
            report.AppendLine(title);
            if (samples.Count == 0) report.AppendLine("- none");
            foreach (string sample in samples) report.AppendLine(sample);
        }
    }
}
#endif
