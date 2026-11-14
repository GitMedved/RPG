#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Creates a review-only scene of real Kenney environment models that may fit the Mia & Max backyard.
    /// Imported source models and production assets are not modified.
    /// </summary>
    public static class BackyardEnvironmentCandidateReviewBuilder
    {
        private const string ModelRoot = "Assets/_Project/Art/Environment/Runtime/Models";
        private const string SceneRoot = "Assets/_Project/Scenes/ArtReview";
        private const string PreviewMaterialsRoot = "Assets/_Project/Art/Environment/Runtime/ReviewMaterials/BackyardCandidates";
        private const int MaxPerCategory = 6;
        private const float CellWidth = 3.0f;
        private const float RowDepth = 4.2f;

        private sealed class Category
        {
            public string Title;
            public string[] Tokens;
            public List<string> Paths = new List<string>();
        }

        [MenuItem("Tools/Mia & Max/Assets/Build Backyard Environment Candidate Review")]
        public static void Build()
        {
            Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
            if (urpLit == null)
            {
                EditorUtility.DisplayDialog("Backyard Candidate Review",
                    "URP/Lit shader was not found. Check the active render pipeline.", "OK");
                return;
            }

            var categories = new List<Category>
            {
                new Category { Title = "TREES & LARGE GREENERY", Tokens = new[] { "tree", "pine", "willow", "palm", "oak", "birch", "maple", "bush", "shrub", "hedge" } },
                new Category { Title = "FLOWERS & SMALL PLANTS", Tokens = new[] { "flower", "fern", "grass", "cattail", "reed", "mushroom", "plant", "planter" } },
                new Category { Title = "FENCES & GARDEN STRUCTURES", Tokens = new[] { "fence", "gate", "arch", "pergola", "shed", "birdhouse", "well", "trellis", "garden" } },
                new Category { Title = "NATURAL ROCKS & TERRAIN", Tokens = new[] { "rock", "pebble", "gravel", "stone", "cliff", "stump", "log" } },
                new Category { Title = "OUTDOOR SEATING & DECOR", Tokens = new[] { "swing", "sandbox", "slide", "wheelbarrow", "bench", "birdhouse", "basket", "outdoor", "garden", "chair", "table" } }
            };

            string[] guids = AssetDatabase.FindAssets("t:Model", new[] { ModelRoot });
            var allPaths = guids.Select(AssetDatabase.GUIDToAssetPath)
                .Where(p => !string.IsNullOrEmpty(p))
                .Where(p => !p.Contains("/GLB format/"))
                .OrderBy(p => FormatPriority(p))
                .ThenBy(p => p, StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (Category category in categories)
            {
                var selected = new List<string>();
                foreach (string assetPath in allPaths)
                {
                    string file = Path.GetFileNameWithoutExtension(assetPath).ToLowerInvariant();
                    if (!IsSuitableCandidate(category.Title, file)) continue;

                    GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    if (model == null || model.GetComponentsInChildren<Renderer>(true).Length == 0) continue;

                    string normalized = file;
                    if (selected.Any(existing =>
                        Path.GetFileNameWithoutExtension(existing).Equals(normalized, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    selected.Add(assetPath);
                    if (selected.Count >= MaxPerCategory) break;
                }
                category.Paths = selected;
            }

            int total = categories.Sum(c => c.Paths.Count);
            if (total == 0)
            {
                EditorUtility.DisplayDialog("Backyard Candidate Review",
                    "No matching environment models were found. Check the imported model folder.", "OK");
                return;
            }

            EnsureFolders();
            string scenePath = NextScenePath();
            Scene previous = SceneManager.GetActiveScene();
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            scene.name = "BackyardEnvironmentCandidateReview";
            SceneManager.SetActiveScene(scene);

            CreateLighting();
            CreateGround(categories.Count);
            int row = 0;
            foreach (Category category in categories)
            {
                if (category.Paths.Count > 0)
                    CreateCategory(category, row++, scene, urpLit);
            }
            CreateCamera(row);

            EditorSceneManager.MarkSceneDirty(scene);
            bool saved = EditorSceneManager.SaveScene(scene, scenePath);
            if (previous.IsValid() && previous.isLoaded) SceneManager.SetActiveScene(previous);
            AssetDatabase.Refresh();

            string summary = string.Join("\n", categories.Where(c => c.Paths.Count > 0)
                .Select(c => c.Title + ": " + c.Paths.Count));
            if (saved)
            {
                Debug.Log("[Mia & Max] Backyard candidate review created. " + total +
                    " real model candidates.\n" + summary + "\nScene: " + scenePath +
                    "\nOriginal imported models and production prefabs were not modified.");
                EditorUtility.DisplayDialog("Backyard Candidate Review",
                    "Created a review scene with " + total + " real model candidates.\n\n" +
                    summary + "\n\n" + scenePath +
                    "\n\nReview candidates only; no production approval or prefab integration was performed.",
                    "OK");
            }
            else Debug.LogError("[Mia & Max] Could not save candidate review scene: " + scenePath);
        }

        [MenuItem("Tools/Mia & Max/Assets/Print Backyard Candidate Catalog")]
        public static void PrintCandidateCatalog()
        {
            var categories = new[]
            {
                new { Title = "TREES & LARGE GREENERY", Tokens = new[] { "tree", "pine", "willow", "palm", "oak", "birch", "maple", "bush", "shrub", "hedge" } },
                new { Title = "FLOWERS & SMALL PLANTS", Tokens = new[] { "flower", "fern", "grass", "cattail", "reed", "mushroom", "plant", "planter" } },
                new { Title = "FENCES & GARDEN STRUCTURES", Tokens = new[] { "fence", "gate", "arch", "pergola", "shed", "birdhouse", "well", "trellis", "garden" } },
                new { Title = "NATURAL ROCKS & TERRAIN", Tokens = new[] { "rock", "pebble", "gravel", "stone", "cliff", "stump", "log" } },
                new { Title = "OUTDOOR SEATING & DECOR", Tokens = new[] { "swing", "sandbox", "slide", "wheelbarrow", "bench", "birdhouse", "basket", "outdoor", "garden", "chair", "table" } }
            };

            string[] guids = AssetDatabase.FindAssets("t:Model", new[] { ModelRoot });
            var paths = guids.Select(AssetDatabase.GUIDToAssetPath)
                .Where(p => !string.IsNullOrEmpty(p) && !p.Contains("/GLB format/"))
                .OrderBy(p => FormatPriority(p))
                .ThenBy(p => p, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var lines = new List<string> { "[Mia & Max] BACKYARD CANDIDATE CATALOG" };
            int total = 0;
            foreach (var category in categories)
            {
                var selected = new List<string>();
                foreach (string path in paths)
                {
                    string file = Path.GetFileNameWithoutExtension(path).ToLowerInvariant();
                    if (!IsSuitableCandidate(category.Title, file)) continue;
                    GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (model == null || model.GetComponentsInChildren<Renderer>(true).Length == 0) continue;
                    if (selected.Any(existing => Path.GetFileNameWithoutExtension(existing)
                        .Equals(file, StringComparison.OrdinalIgnoreCase))) continue;
                    selected.Add(path);
                    if (selected.Count >= MaxPerCategory) break;
                }

                lines.Add("");
                lines.Add(category.Title + " (" + selected.Count + ")");
                for (int i = 0; i < selected.Count; i++)
                {
                    string path = selected[i];
                    GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    Renderer[] renderers = model.GetComponentsInChildren<Renderer>(true);
                    int materialSlots = renderers.Sum(x => x.sharedMaterials.Length);
                    lines.Add("  " + (i + 1) + ". " + Path.GetFileNameWithoutExtension(path));
                    lines.Add("     " + path + " | renderers: " + renderers.Length + " | material slots: " + materialSlots);
                    total++;
                }
            }

            lines.Insert(1, "Total: " + total + " | Review scene: " + FindLatestScenePath());
            Debug.Log(string.Join("\n", lines));
            EditorUtility.DisplayDialog("Backyard Candidate Catalog",
                "Printed " + total + " candidate names and asset paths to the Unity Console.\n\n" +
                "Use the Console search field with 'BACKYARD CANDIDATE CATALOG' to find the report.",
                "OK");
        }

        private static bool IsSuitableCandidate(string category, string file)
        {
            bool hasAny(params string[] tokens) => tokens.Any(token => file.Contains(token));

            if (category == "TREES & LARGE GREENERY")
            {
                if (hasAny("bush", "shrub", "hedge")) return false;
                return hasAny("tree", "pine", "willow", "palm", "oak", "birch", "maple");
            }

            if (category == "FLOWERS & SMALL PLANTS")
            {
                if (hasAny("potted", "pot", "furniture", "cabinet")) return false;
                return hasAny("flower", "fern", "grass", "cattail", "reed", "mushroom", "plant", "planter");
            }

            if (category == "FENCES & GARDEN STRUCTURES")
                return hasAny("fence", "gate", "arch", "pergola", "shed", "birdhouse", "well", "trellis", "garden");

            if (category == "NATURAL ROCKS & TERRAIN")
            {
                if (hasAny("bridge", "campfire")) return false;
                return hasAny("rock", "pebble", "gravel", "stone", "cliff", "stump", "log");
            }

            if (category == "OUTDOOR SEATING & DECOR")
            {
                if (hasAny("cabinet", "bed", "drawer", "wardrobe", "shelf", "desk", "sofa", "couch", "indoor", "plantstand", "nightstand"))
                    return false;
                return hasAny("swing", "sandbox", "slide", "wheelbarrow", "bench", "birdhouse", "basket", "outdoor", "garden", "chair", "table");
            }

            return false;
        }

        private static int FormatPriority(string path)
        {
            string lower = path.ToLowerInvariant();
            if (lower.EndsWith(".fbx")) return 0;
            if (lower.EndsWith(".obj")) return 1;
            if (lower.EndsWith(".dae")) return 2;
            return 3;
        }

        private static void CreateCategory(Category category, int row, Scene scene, Shader urpLit)
        {
            float z = -row * RowDepth;
            CreateLabel(category.Title, new Vector3(-7.0f, 3.0f, z), 0.24f, scene);

            for (int i = 0; i < category.Paths.Count; i++)
            {
                string assetPath = category.Paths[i];
                GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (source == null) continue;

                GameObject instance = PrefabUtility.InstantiatePrefab(source, scene) as GameObject;
                if (instance == null) instance = UnityEngine.Object.Instantiate(source);
                instance.name = "REVIEW_ONLY_" + Path.GetFileNameWithoutExtension(assetPath);
                if (instance.scene != scene) SceneManager.MoveGameObjectToScene(instance, scene);

                Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
                Bounds bounds = CalculateBounds(renderers, instance.transform.position);
                Vector3 center = bounds.center;
                instance.transform.position -= center;

                float maxDimension = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z, 0.001f);
                float scale = 1.65f / maxDimension;
                instance.transform.localScale *= scale;

                float x = (i - (MaxPerCategory - 1) * 0.5f) * CellWidth;
                instance.transform.position += new Vector3(x, bounds.extents.y * scale + 0.12f, z);

                foreach (Renderer renderer in renderers)
                {
                    Material[] materials = renderer.sharedMaterials;
                    for (int m = 0; m < materials.Length; m++)
                    {
                        if (materials[m] != null)
                            materials[m] = GetPreviewMaterial(materials[m], urpLit);
                    }
                    renderer.sharedMaterials = materials;
                }

                string label = Path.GetFileNameWithoutExtension(assetPath);
                CreateLabel(label, new Vector3(x, 0.04f, z + 1.45f), 0.11f, scene);
                CreateLabel(Path.GetDirectoryName(assetPath).Replace(ModelRoot, "Kenney"), 
                    new Vector3(x, 0.04f, z + 1.2f), 0.065f, scene);
            }
        }

        private static Bounds CalculateBounds(Renderer[] renderers, Vector3 fallback)
        {
            bool hasBounds = false;
            Bounds bounds = new Bounds(fallback, Vector3.one);
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null) continue;
                if (!hasBounds) { bounds = renderer.bounds; hasBounds = true; }
                else bounds.Encapsulate(renderer.bounds);
            }
            return bounds;
        }

        private static Material GetPreviewMaterial(Material source, Shader urpLit)
        {
            if (source.shader != null && source.shader.name == "Universal Render Pipeline/Lit")
                return source;

            string identity = AssetDatabase.GetAssetPath(source) + "|" + source.name + "|" + source.shader;
            string key = StableKey(identity);
            string path = PreviewMaterialsRoot + "/" + SafeName(source.name) + "_" + key + ".mat";
            Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) return existing;

            Material result = new Material(urpLit) { name = source.name + "_BackyardReview_URP" };
            Color color = source.HasProperty("_BaseColor") ? source.GetColor("_BaseColor") :
                source.HasProperty("_Color") ? source.GetColor("_Color") : Color.white;
            result.SetColor("_BaseColor", color);

            CopyTexture(source, result, "_MainTex", "_BaseMap");
            CopyTexture(source, result, "_BaseMap", "_BaseMap");
            CopyTexture(source, result, "_BumpMap", "_BumpMap");
            CopyTexture(source, result, "_MetallicGlossMap", "_MetallicGlossMap");
            CopyTexture(source, result, "_OcclusionMap", "_OcclusionMap");
            CopyTexture(source, result, "_EmissionMap", "_EmissionMap");

            if (source.HasProperty("_Metallic")) result.SetFloat("_Metallic", source.GetFloat("_Metallic"));
            if (source.HasProperty("_Glossiness")) result.SetFloat("_Smoothness", source.GetFloat("_Glossiness"));
            if (source.HasProperty("_BumpScale")) result.SetFloat("_BumpScale", source.GetFloat("_BumpScale"));

            if (source.HasProperty("_EmissionColor") &&
                (source.IsKeywordEnabled("_EMISSION") || source.GetTexture("_EmissionMap") != null))
            {
                result.SetColor("_EmissionColor", source.GetColor("_EmissionColor"));
                result.EnableKeyword("_EMISSION");
            }

            bool alphaClip = source.HasProperty("_Mode") && Mathf.Approximately(source.GetFloat("_Mode"), 1f);
            bool transparent = color.a < 0.999f ||
                (source.HasProperty("_Mode") && source.GetFloat("_Mode") >= 2f);
            if (alphaClip)
            {
                result.SetFloat("_AlphaClip", 1f);
                result.EnableKeyword("_ALPHATEST_ON");
                if (source.HasProperty("_Cutoff")) result.SetFloat("_Cutoff", source.GetFloat("_Cutoff"));
            }
            else if (transparent)
            {
                result.SetFloat("_Surface", 1f);
                result.SetFloat("_ZWrite", 0f);
                result.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                result.renderQueue = (int)RenderQueue.Transparent;
            }

            AssetDatabase.CreateAsset(result, path);
            return result;
        }

        private static void CopyTexture(Material source, Material destination, string from, string to)
        {
            if (!source.HasProperty(from) || !destination.HasProperty(to)) return;
            Texture texture = source.GetTexture(from);
            if (texture == null) return;
            destination.SetTexture(to, texture);
            destination.SetTextureScale(to, source.GetTextureScale(from));
            destination.SetTextureOffset(to, source.GetTextureOffset(from));
        }

        private static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/_Project/Art/Environment/Runtime/ReviewMaterials"))
                AssetDatabase.CreateFolder("Assets/_Project/Art/Environment/Runtime", "ReviewMaterials");
            if (!AssetDatabase.IsValidFolder(PreviewMaterialsRoot))
                AssetDatabase.CreateFolder("Assets/_Project/Art/Environment/Runtime/ReviewMaterials", "BackyardCandidates");
            if (!AssetDatabase.IsValidFolder(SceneRoot))
                AssetDatabase.CreateFolder("Assets/_Project/Scenes", "ArtReview");
        }

        private static string FindLatestScenePath()
        {
            if (!Directory.Exists(SceneRoot)) return "No review scene found yet";
            string latest = Directory.GetFiles(SceneRoot, "BackyardEnvironmentCandidates*.unity")
                .OrderByDescending(File.GetLastWriteTimeUtc)
                .FirstOrDefault();
            return string.IsNullOrEmpty(latest)
                ? "No review scene found yet"
                : latest.Replace('\\\\', '/');
        }

        private static string NextScenePath()
        {
            string path = SceneRoot + "/BackyardEnvironmentCandidates.unity";
            if (!File.Exists(path)) return path;
            int n = 2;
            do { path = SceneRoot + "/BackyardEnvironmentCandidates_" + n.ToString("D2") + ".unity"; n++; }
            while (File.Exists(path));
            return path;
        }

        private static string StableKey(string value)
        {
            unchecked
            {
                uint hash = 2166136261;
                foreach (char c in value) hash = (hash ^ c) * 16777619;
                return hash.ToString("X8");
            }
        }

        private static string SafeName(string value)
        {
            foreach (char c in Path.GetInvalidFileNameChars()) value = value.Replace(c, '_');
            return value.Replace('/', '_').Replace('\\', '_').Replace(' ', '_');
        }

        private static void CreateGround(int rows)
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "REVIEW_ONLY_BackyardCandidateGround";
            ground.transform.position = new Vector3(0f, -0.08f, -(rows - 1) * RowDepth * 0.5f);
            ground.transform.localScale = new Vector3(2.8f, 1f, Mathf.Max(1f, rows * 0.75f));
            const string groundMaterialPath = PreviewMaterialsRoot + "/CandidateGround.mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(groundMaterialPath);
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                material.name = "REVIEW_ONLY_BackyardCandidateGroundMaterial";
                material.SetColor("_BaseColor", new Color(0.25f, 0.38f, 0.24f));
                AssetDatabase.CreateAsset(material, groundMaterialPath);
            }
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
            fill.intensity = 0.5f;
            fillObject.transform.rotation = Quaternion.Euler(25f, 145f, 0f);
        }

        private static void CreateLabel(string value, Vector3 position, float size, Scene scene)
        {
            GameObject label = new GameObject("REVIEW_ONLY_Label_" + SafeName(value));
            SceneManager.MoveGameObjectToScene(label, scene);
            label.transform.position = position;
            TextMesh text = label.AddComponent<TextMesh>();
            text.text = value;
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.fontSize = 36;
            text.characterSize = size;
            text.color = Color.white;
        }

        private static void CreateCamera(int rows)
        {
            GameObject cameraObject = new GameObject("REVIEW_ONLY_CandidateCamera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.15f, 0.19f, 0.16f);
            camera.orthographic = true;
            camera.orthographicSize = Mathf.Max(8f, rows * 2.4f);
            cameraObject.transform.position = new Vector3(0f, 15f, 2f);
            cameraObject.transform.LookAt(new Vector3(0f, 0.4f, -(rows - 1) * RowDepth * 0.5f));
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 200f;
            camera.tag = "MainCamera";
        }
    }
}
#endif
