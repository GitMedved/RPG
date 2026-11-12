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
        private const int MaxSamples = 24;

        [MenuItem("Tools/Mia & Max/Assets/Diagnose Environment Materials")]
        public static void Run()
        {
            AssetDatabase.Refresh();

            string[] modelGuids = AssetDatabase.FindAssets("t:Model", new[] { ModelRoot });
            string[] materialGuids = AssetDatabase.FindAssets("t:Material", new[] { ModelRoot });
            string[] textureGuids = AssetDatabase.FindAssets("t:Texture", new[] { ModelRoot });

            var materials = new Dictionary<string, Material>(StringComparer.OrdinalIgnoreCase);
            int modelsScanned = 0;
            int renderersScanned = 0;
            int meshesWithVertexColors = 0;
            int meshesWithoutVertexColors = 0;
            int rendererSlots = 0;
            int emptySlots = 0;

            foreach (string guid in modelGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (UnityEngine.Object asset in assets)
                {
                    Material embedded = asset as Material;
                    if (embedded != null)
                        materials[AssetDatabase.GetAssetPath(embedded) + "::" + embedded.name] = embedded;

                    Mesh mesh = asset as Mesh;
                    if (mesh != null)
                    {
                        if (mesh.colors != null && mesh.colors.Length == mesh.vertexCount)
                            meshesWithVertexColors++;
                        else
                            meshesWithoutVertexColors++;
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
                        if (material == null)
                        {
                            emptySlots++;
                            continue;
                        }
                        materials[AssetDatabase.GetAssetPath(material) + "::" + material.name] = material;
                    }
                }
            }

            int materialsWithTextures = 0;
            int materialsWithBaseMap = 0;
            int materialsWithNonWhiteColor = 0;
            var shaderCounts = new SortedDictionary<string, int>();
            var sampleLines = new List<string>();

            foreach (Material material in materials.Values)
            {
                string shaderName = material.shader != null ? material.shader.name : "<missing shader>";
                if (!shaderCounts.ContainsKey(shaderName)) shaderCounts[shaderName] = 0;
                shaderCounts[shaderName]++;

                bool hasTexture = false;
                string textureProperties = "";
                try
                {
                    foreach (string propertyName in material.GetTexturePropertyNames())
                    {
                        Texture texture = material.GetTexture(propertyName);
                        if (texture == null) continue;
                        hasTexture = true;
                        textureProperties += (textureProperties.Length == 0 ? "" : ", ") + propertyName + "=" + texture.name;
                        if (propertyName == "_BaseMap" || propertyName == "_MainTex" || propertyName == "baseColorTexture")
                            materialsWithBaseMap++;
                    }
                }
                catch (Exception) { }

                if (hasTexture) materialsWithTextures++;
                Color color = material.HasProperty("_BaseColor") ? material.GetColor("_BaseColor") :
                    material.HasProperty("_Color") ? material.GetColor("_Color") : Color.white;
                if (Mathf.Abs(color.r - 1f) > 0.001f || Mathf.Abs(color.g - 1f) > 0.001f || Mathf.Abs(color.b - 1f) > 0.001f || Mathf.Abs(color.a - 1f) > 0.001f)
                    materialsWithNonWhiteColor++;

                if (sampleLines.Count < MaxSamples)
                    sampleLines.Add("- " + material.name + " | shader: " + shaderName +
                        " | color: " + ColorUtility.ToHtmlStringRGBA(color) +
                        " | textures: " + (hasTexture ? textureProperties : "none"));
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
            foreach (var pair in shaderCounts)
                report.AppendLine("- " + pair.Key + ": " + pair.Value);
            report.AppendLine();
            report.AppendLine("MATERIAL SAMPLES:");
            foreach (string line in sampleLines)
                report.AppendLine(line);
            report.AppendLine();
            report.AppendLine("Read-only diagnostic. No assets were changed.");

            Debug.Log(report.ToString());
            EditorUtility.DisplayDialog("Mia & Max — Material Diagnostics",
                "Models: " + modelGuids.Length +
                "\nUnique materials: " + materials.Count +
                "\nMaterials with textures: " + materialsWithTextures +
                "\nTexture assets: " + textureGuids.Length +
                "\nVertex-color meshes: " + meshesWithVertexColors +
                "\n\nFull diagnostic report is in Console. This tool does not modify assets.",
                "OK");
        }
    }
}
#endif
