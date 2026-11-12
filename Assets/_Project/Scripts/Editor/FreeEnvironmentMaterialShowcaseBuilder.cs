#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Builds a compact, disposable material showcase from imported environment assets.
    /// Source models, embedded materials, textures and production scenes are never modified.
    /// </summary>
    public static class FreeEnvironmentMaterialShowcaseBuilder
    {
        private const string ModelRoot = "Assets/_Project/Art/Environment/Runtime/Models";
        private const string SceneDirectory = "Assets/_Project/Scenes/ArtReview";
        private const string MaterialDirectory = "Assets/_Project/Art/Environment/Runtime/ReviewMaterials/Showcase";
        private const int SamplesPerCategory = 6;
        private const float ColumnSpacing = 2.45f;
        private const float RowSpacing = 3.35f;

        private sealed class MaterialSample
        {
            public Material Source;
            public string AssetPath;
            public bool HasAlbedo;
            public bool IsColored;
        }

        [MenuItem("Tools/Mia & Max/Assets/Build Environment Material Showcase")]
        public static void Build()
        {
            if (!Directory.Exists(ModelRoot))
            {
                EditorUtility.DisplayDialog("Mia & Max Material Showcase",
                    "Environment model folder was not found. Import the free asset packs first.", "OK");
                return;
            }

            Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
            if (urpLit == null)
            {
                EditorUtility.DisplayDialog("Mia & Max Material Showcase",
                    "URP/Lit shader was not found. Check the active render pipeline before building the showcase.", "OK");
                return;
            }

            List<MaterialSample> samples = CollectSamples();
            List<MaterialSample> textured = samples.Where(x => x.HasAlbedo).Take(SamplesPerCategory).ToList();
            List<MaterialSample> colored = samples.Where(x => !x.HasAlbedo && x.IsColored).Take(SamplesPerCategory).ToList();
            List<MaterialSample> plain = samples.Where(x => !x.HasAlbedo && !x.IsColored).Take(SamplesPerCategory).ToList();

            if (textured.Count + colored.Count + plain.Count == 0)
            {
                EditorUtility.DisplayDialog("Mia & Max Material Showcase",
                    "No imported materials were found under the environment model folder.", "OK");
                return;
            }

            EnsureFolders();
            string scenePath = GetNextScenePath();
            Scene previousActiveScene = SceneManager.GetActiveScene();
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            scene.name = "EnvironmentMaterialShowcase";
            SceneManager.SetActiveScene(scene);

            CreateLighting();
            CreateGround();
            int shown = 0;
            shown += CreateCategory(textured, "TEXTURED / ATLAS MATERIALS", 0, scene, urpLit);
            shown += CreateCategory(colored, "COLOR-ONLY MATERIALS", 1, scene, urpLit);
            shown += CreateCategory(plain, "WHITE / DEFAULT MATERIALS", 2, scene, urpLit);
            CreateCamera();

            EditorSceneManager.MarkSceneDirty(scene);
            bool saved = EditorSceneManager.SaveScene(scene, scenePath);
            if (previousActiveScene.IsValid() && previousActiveScene.isLoaded)
                SceneManager.SetActiveScene(previousActiveScene);
            AssetDatabase.Refresh();

            if (saved)
            {
                Debug.Log("[Mia & Max] Environment material showcase created. Samples: " + shown +
                    " (textured: " + textured.Count + ", color-only: " + colored.Count +
                    ", white/default: " + plain.Count + "). Scene: " + scenePath +
                    ". Source assets were not modified.");
                EditorUtility.DisplayDialog("Mia & Max Material Showcase",
                    "Created " + shown + " material samples in a compact review scene:\n" + scenePath +
                    "\n\nTextured: " + textured.Count +
                    "\nColor-only: " + colored.Count +
                    "\nWhite/default: " + plain.Count +
                    "\n\nOnly preview copies are generated for legacy Standard materials. Source assets remain unchanged.",
                    "OK");
            }
            else
            {
                Debug.LogError("[Mia & Max] Failed to save material showcase scene: " + scenePath);
            }
        }

        private static List<MaterialSample> CollectSamples()
        {
            var unique = new Dictionary<string, MaterialSample>(StringComparer.OrdinalIgnoreCase);
            string[] modelGuids = AssetDatabase.FindAssets("t:Model", new[] { ModelRoot });
            string[] materialGuids = AssetDatabase.FindAssets("t:Material", new[] { ModelRoot });

            foreach (string guid in modelGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                foreach (UnityEngine.Object asset in AssetDatabase.LoadAllAssetsAtPath(path))
                {
                    Material material = asset as Material;
                    if (material != null) AddMaterial(unique, material, path);
                }
            }

            foreach (string guid in materialGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (material != null) AddMaterial(unique, material, path);
            }

            return unique.Values.OrderBy(x => x.AssetPath, StringComparer.OrdinalIgnoreCase)
                .ThenBy(x => x.Source.name, StringComparer.OrdinalIgnoreCase).ToList();
        }

        private static void AddMaterial(Dictionary<string, MaterialSample> unique, Material material, string path)
        {
            string key = path + "::" + material.name;
            if (unique.ContainsKey(key)) return;

            bool hasAlbedo = false;
            try
            {
                foreach (string property in material.GetTexturePropertyNames())
                {
                    if (property != "_BaseMap" && property != "_MainTex" && property != "baseColorTexture")
                        continue;
                    if (material.GetTexture(property) != null) hasAlbedo = true;
                }
            }
            catch (Exception) { }

            Color color = GetMaterialColor(material);
            bool colored = Mathf.Abs(color.r - 1f) > 0.002f ||
                Mathf.Abs(color.g - 1f) > 0.002f || Mathf.Abs(color.b - 1f) > 0.002f ||
                Mathf.Abs(color.a - 1f) > 0.002f;

            unique.Add(key, new MaterialSample
            {
                Source = material,
                AssetPath = path,
                HasAlbedo = hasAlbedo,
                IsColored = colored
            });
        }

        private static int CreateCategory(List<MaterialSample> samples, string title, int category,
            Scene scene, Shader urpLit)
        {
            if (samples.Count == 0) return 0;

            float z = -category * RowSpacing;
            CreateLabel(title, new Vector3(-6.1f, 2.15f, z + 0.2f), 0.22f);

            for (int i = 0; i < samples.Count; i++)
            {
                float x = (i - (SamplesPerCategory - 1) * 0.5f) * ColumnSpacing;
                Vector3 position = new Vector3(x, 0.85f, z);

                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.name = "REVIEW_ONLY_" + SafeName(samples[i].Source.name);
                SceneManager.MoveGameObjectToScene(sphere, scene);
                sphere.transform.position = position;
                sphere.transform.localScale = Vector3.one * 1.25f;

                Material previewMaterial = GetPreviewMaterial(samples[i].Source, urpLit);
                sphere.GetComponent<Renderer>().sharedMaterial = previewMaterial;

                string shortName = samples[i].Source.name;
                if (shortName.Length > 22) shortName = shortName.Substring(0, 22);
                CreateLabel(shortName, position + new Vector3(0f, -0.95f, 0f), 0.12f);
                CreateLabel(Path.GetFileName(samples[i].AssetPath), position + new Vector3(0f, -1.2f, 0f), 0.075f);
            }

            return samples.Count;
        }

        private static Material GetPreviewMaterial(Material source, Shader urpLit)
        {
            if (source.shader != null && source.shader.name == "Universal Render Pipeline/Lit")
                return source;

            string key = StableKey(AssetDatabase.GetAssetPath(source) + "|" + source.name);
            string path = MaterialDirectory + "/" + SafeName(source.name) + "_" + key + ".mat";
            Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) return existing;

            Material result = new Material(urpLit);
            result.name = source.name + "_Showcase_URP";
            Color color = GetMaterialColor(source);
            result.SetColor("_BaseColor", color);

            CopyTexture(source, result, "_MainTex", "_BaseMap");
            CopyTexture(source, result, "_BaseMap", "_BaseMap");
            CopyTexture(source, result, "_BumpMap", "_BumpMap");
            CopyTexture(source, result, "_MetallicGlossMap", "_MetallicGlossMap");
            CopyTexture(source, result, "_OcclusionMap", "_OcclusionMap");
            CopyTexture(source, result, "_EmissionMap", "_EmissionMap");

            if (source.HasProperty("_Metallic") && result.HasProperty("_Metallic"))
                result.SetFloat("_Metallic", source.GetFloat("_Metallic"));
            if (source.HasProperty("_Glossiness") && result.HasProperty("_Smoothness"))
                result.SetFloat("_Smoothness", source.GetFloat("_Glossiness"));
            if (source.HasProperty("_BumpScale") && result.HasProperty("_BumpScale"))
                result.SetFloat("_BumpScale", source.GetFloat("_BumpScale"));
            if (source.HasProperty("_OcclusionStrength") && result.HasProperty("_OcclusionStrength"))
                result.SetFloat("_OcclusionStrength", source.GetFloat("_OcclusionStrength"));

            if (source.IsKeywordEnabled("_EMISSION") || source.GetTexture("_EmissionMap") != null)
            {
                result.EnableKeyword("_EMISSION");
                if (source.HasProperty("_EmissionColor") && result.HasProperty("_EmissionColor"))
                    result.SetColor("_EmissionColor", source.GetColor("_EmissionColor"));
            }

            bool cutout = source.HasProperty("_Mode") && Mathf.Approximately(source.GetFloat("_Mode"), 1f);
            bool transparent = color.a < 0.999f ||
                (source.HasProperty("_Mode") && source.GetFloat("_Mode") >= 2f);
            if (cutout)
            {
                result.SetFloat("_Surface", 0f);
                result.SetFloat("_AlphaClip", 1f);
                result.EnableKeyword("_ALPHATEST_ON");
                if (source.HasProperty("_Cutoff")) result.SetFloat("_Cutoff", source.GetFloat("_Cutoff"));
            }
            else if (transparent)
            {
                result.SetFloat("_Surface", 1f);
                result.SetFloat("_Blend", 0f);
                result.SetFloat("_ZWrite", 0f);
                result.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                result.renderQueue = (int)RenderQueue.Transparent;
            }

            AssetDatabase.CreateAsset(result, path);
            return result;
        }

        private static void CopyTexture(Material source, Material destination, string sourceProperty, string destinationProperty)
        {
            if (!source.HasProperty(sourceProperty) || !destination.HasProperty(destinationProperty)) return;
            Texture texture = source.GetTexture(sourceProperty);
            if (texture == null) return;
            destination.SetTexture(destinationProperty, texture);
            Vector2 scale = source.GetTextureScale(sourceProperty);
            Vector2 offset = source.GetTextureOffset(sourceProperty);
            destination.SetTextureScale(destinationProperty, scale);
            destination.SetTextureOffset(destinationProperty, offset);
        }

        private static Color GetMaterialColor(Material material)
        {
            if (material.HasProperty("_BaseColor")) return material.GetColor("_BaseColor");
            if (material.HasProperty("_Color")) return material.GetColor("_Color");
            return Color.white;
        }

        private static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/_Project/Art/Environment/Runtime/ReviewMaterials"))
                AssetDatabase.CreateFolder("Assets/_Project/Art/Environment/Runtime", "ReviewMaterials");
            if (!AssetDatabase.IsValidFolder(MaterialDirectory))
                AssetDatabase.CreateFolder("Assets/_Project/Art/Environment/Runtime/ReviewMaterials", "Showcase");
            if (!AssetDatabase.IsValidFolder(SceneDirectory))
                AssetDatabase.CreateFolder("Assets/_Project/Scenes", "ArtReview");
        }

        private static string GetNextScenePath()
        {
            string first = SceneDirectory + "/EnvironmentMaterialShowcase.unity";
            if (!File.Exists(first)) return first;
            int version = 2;
            string candidate;
            do
            {
                candidate = SceneDirectory + "/EnvironmentMaterialShowcase_" + version.ToString("D2") + ".unity";
                version++;
            } while (File.Exists(candidate));
            return candidate;
        }

        private static string SafeName(string value)
        {
            foreach (char c in Path.GetInvalidFileNameChars()) value = value.Replace(c, '_');
            return value.Replace('/', '_').Replace('\\', '_').Replace(' ', '_');
        }

        private static string StableKey(string value)
        {
            unchecked
            {
                uint hash = 2166136261;
                for (int i = 0; i < value.Length; i++) hash = (hash ^ value[i]) * 16777619;
                return hash.ToString("X8");
            }
        }

        private static void CreateGround()
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "REVIEW_ONLY_ShowcaseGround";
            ground.transform.position = new Vector3(0f, -0.04f, -RowSpacing);
            ground.transform.localScale = new Vector3(2.2f, 1f, 1.35f);
            Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.name = "REVIEW_ONLY_ShowcaseGroundMaterial";
            material.SetColor("_BaseColor", new Color(0.27f, 0.32f, 0.28f));
            ground.GetComponent<Renderer>().sharedMaterial = material;
        }

        private static void CreateLighting()
        {
            GameObject keyObject = new GameObject("REVIEW_ONLY_KeyLight");
            Light key = keyObject.AddComponent<Light>();
            key.type = LightType.Directional;
            key.intensity = 1.25f;
            keyObject.transform.rotation = Quaternion.Euler(48f, -28f, 0f);

            GameObject fillObject = new GameObject("REVIEW_ONLY_FillLight");
            Light fill = fillObject.AddComponent<Light>();
            fill.type = LightType.Directional;
            fill.intensity = 0.55f;
            fillObject.transform.rotation = Quaternion.Euler(25f, 145f, 0f);
        }

        private static void CreateLabel(string value, Vector3 position, float characterSize)
        {
            GameObject label = new GameObject("REVIEW_ONLY_Label_" + SafeName(value));
            label.transform.position = position;
            TextMesh text = label.AddComponent<TextMesh>();
            text.text = value;
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.fontSize = 36;
            text.characterSize = characterSize;
            text.color = Color.white;
        }

        private static void CreateCamera()
        {
            GameObject cameraObject = new GameObject("REVIEW_ONLY_ShowcaseCamera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.13f, 0.16f, 0.19f);
            camera.orthographic = true;
            camera.orthographicSize = 7.4f;
            cameraObject.transform.position = new Vector3(0f, 13f, -15f);
            cameraObject.transform.LookAt(new Vector3(0f, 0.5f, -RowSpacing));
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 100f;
            camera.tag = "MainCamera";
        }
    }
}
#endif
