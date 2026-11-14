#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Builds clean, normalized visual prefabs from the reviewed Kenney backyard source models.
    /// Source models are never modified. Output contains no colliders, rigidbodies or behaviours.
    /// </summary>
    public static class BackyardProductionEnvironmentPrefabBuilder
    {
        private const string ModelRoot = "Assets/_Project/Art/Environment/Runtime/Models/Kenney";
        private const string OutputRoot = "Assets/_Project/Art/Environment/Runtime/ProductionPrefabs/Environment";
        private const string ManifestPath = OutputRoot + "/ENVIRONMENT_ASSET_MANIFEST.md";

        private sealed class AssetSpec
        {
            public string Name;
            public float TargetHeight;
            public float TargetWidth;
            public string Role;
            public AssetSpec(string name, float height, float width, string role)
            { Name = name; TargetHeight = height; TargetWidth = width; Role = role; }
        }

        private static readonly AssetSpec[] Specs =
        {
            new AssetSpec("fence_planks", 1.45f, 2.9f, "Straight perimeter fence module"),
            new AssetSpec("fence_gate", 1.8f, 2.1f, "Perimeter gate module"),
            new AssetSpec("fence_corner", 1.5f, 1.6f, "Fence corner module"),
            new AssetSpec("tree_blocks", 5.0f, 0f, "Main broad-canopy tree"),
            new AssetSpec("tree_cone", 4.6f, 0f, "Secondary stylized tree"),
            new AssetSpec("bench", 0.95f, 2.0f, "Outdoor bench"),
            new AssetSpec("chairRounded", 0.95f, 1.0f, "Rounded outdoor chair"),
            new AssetSpec("chairCushion", 0.95f, 1.0f, "Cushioned outdoor chair"),
            new AssetSpec("flower_purpleA", 0.85f, 0.85f, "Flower variation A"),
            new AssetSpec("flower_purpleB", 0.85f, 0.85f, "Flower variation B"),
            new AssetSpec("flower_purpleC", 0.85f, 0.85f, "Flower variation C"),
            new AssetSpec("plantSmall1", 0.85f, 0.85f, "Small plant variation 1"),
            new AssetSpec("plantSmall2", 0.85f, 0.85f, "Small plant variation 2"),
            new AssetSpec("plantSmall3", 0.85f, 0.85f, "Small plant variation 3"),
            new AssetSpec("cliff_blockHalf_stone", 0.7f, 1.5f, "Stone terrain accent"),
            new AssetSpec("cliff_blockDiagonal_rock", 0.8f, 1.3f, "Rock terrain accent"),
            new AssetSpec("cliff_blockHalf_rock", 0.7f, 1.5f, "Rock terrain accent"),
            new AssetSpec("cliff_blockDiagonal_stone", 0.8f, 1.3f, "Stone terrain accent")
        };

        [MenuItem("Tools/Mia & Max/Assets/Build Production Environment Prefabs")]
        public static void Build()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Mia & Max", "Exit Play Mode before building production prefabs.", "OK");
                return;
            }

            EnsureFolder("Assets/_Project/Art/Environment/Runtime/ProductionPrefabs");
            EnsureFolder(OutputRoot);

            string[] guids = AssetDatabase.FindAssets("t:Model", new[] { ModelRoot });
            string[] modelPaths = guids.Select(AssetDatabase.GUIDToAssetPath)
                .Where(p => !string.IsNullOrEmpty(p) && !p.Contains("/GLB format/"))
                .OrderBy(p => p.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                .ThenBy(p => p, StringComparer.OrdinalIgnoreCase).ToArray();

            var missing = new List<string>();
            var errors = new List<string>();
            var rows = new List<string>();
            int built = 0;
            int totalTriangles = 0;

            foreach (AssetSpec spec in Specs)
            {
                string sourcePath = modelPaths.FirstOrDefault(p =>
                    Path.GetFileNameWithoutExtension(p).Equals(spec.Name, StringComparison.OrdinalIgnoreCase));
                if (string.IsNullOrEmpty(sourcePath))
                {
                    missing.Add(spec.Name);
                    continue;
                }

                try
                {
                    GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
                    if (source == null) { missing.Add(spec.Name + " (not loadable)"); continue; }

                    GameObject wrapper = new GameObject("ENV_" + ToPascal(spec.Name));
                    try
                    {
                        GameObject visual = PrefabUtility.InstantiatePrefab(source) as GameObject;
                        if (visual == null) visual = UnityEngine.Object.Instantiate(source);
                        visual.name = "Visual";
                        visual.transform.SetParent(wrapper.transform, false);

                        StripGameplayAndPhysicsComponents(visual);
                        Renderer[] renderers = visual.GetComponentsInChildren<Renderer>(true);
                        if (renderers.Length == 0)
                            throw new InvalidOperationException("Source model has no renderers.");

                        Bounds bounds = GetBoundsInRootSpace(renderers, wrapper.transform);
                        if (bounds.size.y <= 0.001f)
                            throw new InvalidOperationException("Source model has zero height.");

                        float scale = spec.TargetHeight / bounds.size.y;
                        if (spec.TargetWidth > 0f)
                        {
                            float horizontal = Mathf.Max(bounds.size.x, bounds.size.z);
                            if (horizontal > 0.001f) scale = spec.TargetWidth / horizontal;
                        }
                        scale = Mathf.Clamp(scale, 0.001f, 100f);
                        visual.transform.localScale = Vector3.one * scale;

                        bounds = GetBoundsInRootSpace(renderers, wrapper.transform);
                        visual.transform.localPosition = Vector3.up * (-bounds.min.y);
                        bounds = GetBoundsInRootSpace(renderers, wrapper.transform);
                        int triangles = CountTriangles(visual);
                        totalTriangles += triangles;

                        string prefabPath = OutputRoot + "/ENV_" + ToPascal(spec.Name) + ".prefab";
                        GameObject saved = PrefabUtility.SaveAsPrefabAsset(wrapper, prefabPath);
                        if (saved == null) throw new InvalidOperationException("Unity did not save the prefab.");

                        built++;
                        rows.Add("| " + spec.Name + " | " + spec.Role + " | " +
                            bounds.size.x.ToString("0.00") + " x " +
                            bounds.size.y.ToString("0.00") + " x " +
                            bounds.size.z.ToString("0.00") + " m | " +
                            triangles + " | " + sourcePath + " |");
                    }
                    finally { UnityEngine.Object.DestroyImmediate(wrapper); }
                }
                catch (Exception ex) { errors.Add(spec.Name + ": " + ex.Message); }
            }

            string manifest = BuildManifest(rows, missing, errors, built, totalTriangles);
            File.WriteAllText(ManifestPath, manifest, new UTF8Encoding(false));
            AssetDatabase.ImportAsset(ManifestPath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            string report = "[Mia & Max] PRODUCTION ENVIRONMENT PREFAB BUILD\n" +
                "Built: " + built + " / " + Specs.Length + "\n" +
                "Missing: " + missing.Count + "\nErrors: " + errors.Count + "\n" +
                "Triangles across unique prefab assets: " + totalTriangles + "\n" +
                "Output: " + OutputRoot + "\n" +
                (missing.Count > 0 ? "Missing models: " + string.Join(", ", missing.ToArray()) + "\n" : "") +
                (errors.Count > 0 ? "Errors: " + string.Join(" | ", errors.ToArray()) + "\n" : "") +
                "Visual-only prefabs created. Source models and gameplay scenes were not modified.";

            if (errors.Count == 0 && missing.Count == 0) Debug.Log(report);
            else Debug.LogWarning(report);

            EditorUtility.DisplayDialog("Mia & Max — Production Environment Prefabs",
                "Built: " + built + " / " + Specs.Length +
                "\nMissing: " + missing.Count + "\nErrors: " + errors.Count +
                "\n\nOutput: " + OutputRoot +
                "\nManifest: ENVIRONMENT_ASSET_MANIFEST.md" +
                "\n\nReview prefab scale, materials and silhouettes before runtime integration. See Console.",
                "OK");
        }

        private static void StripGameplayAndPhysicsComponents(GameObject root)
        {
            foreach (Collider c in root.GetComponentsInChildren<Collider>(true))
                UnityEngine.Object.DestroyImmediate(c);
            foreach (Rigidbody rb in root.GetComponentsInChildren<Rigidbody>(true))
                UnityEngine.Object.DestroyImmediate(rb);
            foreach (MonoBehaviour behaviour in root.GetComponentsInChildren<MonoBehaviour>(true))
                UnityEngine.Object.DestroyImmediate(behaviour);
        }

        private static Bounds GetBoundsInRootSpace(Renderer[] renderers, Transform root)
        {
            bool hasPoint = false;
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

            foreach (Renderer renderer in renderers)
            {
                Mesh mesh = null;
                SkinnedMeshRenderer skinned = renderer as SkinnedMeshRenderer;
                if (skinned != null) mesh = skinned.sharedMesh;

                if (mesh == null)
                {
                    MeshFilter filter = renderer.GetComponent<MeshFilter>();
                    if (filter != null) mesh = filter.sharedMesh;
                }

                Bounds localBounds = mesh != null ? mesh.bounds : renderer.localBounds;
                Matrix4x4 toRoot = root.worldToLocalMatrix * renderer.transform.localToWorldMatrix;
                Vector3 min = localBounds.min;
                Vector3 max = localBounds.max;

                for (int x = 0; x < 2; x++)
                for (int y = 0; y < 2; y++)
                for (int z = 0; z < 2; z++)
                {
                    Vector3 corner = new Vector3(
                        x == 0 ? min.x : max.x,
                        y == 0 ? min.y : max.y,
                        z == 0 ? min.z : max.z);
                    Vector3 point = toRoot.MultiplyPoint3x4(corner);
                    if (!hasPoint)
                    {
                        bounds = new Bounds(point, Vector3.zero);
                        hasPoint = true;
                    }
                    else bounds.Encapsulate(point);
                }
            }

            return bounds;
        }

        private static int CountTriangles(GameObject root)
        {
            int count = 0;
            foreach (MeshFilter f in root.GetComponentsInChildren<MeshFilter>(true))
            {
                Mesh m = f.sharedMesh;
                if (m == null) continue;
                for (int i = 0; i < m.subMeshCount; i++)
                    if (m.GetTopology(i) == MeshTopology.Triangles)
                        count += (int)m.GetIndexCount(i) / 3;
            }
            foreach (SkinnedMeshRenderer r in root.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                Mesh m = r.sharedMesh;
                if (m == null) continue;
                for (int i = 0; i < m.subMeshCount; i++)
                    if (m.GetTopology(i) == MeshTopology.Triangles)
                        count += (int)m.GetIndexCount(i) / 3;
            }
            return count;
        }

        private static string BuildManifest(List<string> rows, List<string> missing,
            List<string> errors, int built, int triangles)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# Mia & Max — Production Environment Prefabs");
            sb.AppendLine();
            sb.AppendLine("Generated by Unity Editor: Build Production Environment Prefabs.");
            sb.AppendLine("Source pack: Kenney model imports. Verify the included pack license file before release; the project integration notes identify the selected Kenney packs as CC0.");
            sb.AppendLine("These are normalized visual prefabs only. Gameplay interaction, navigation and collision belong to separate gameplay roots.");
            sb.AppendLine();
            sb.AppendLine("## Build summary");
            sb.AppendLine("- Built: " + built + " / " + Specs.Length);
            sb.AppendLine("- Missing: " + missing.Count);
            sb.AppendLine("- Errors: " + errors.Count);
            sb.AppendLine("- Total triangles across unique prefab assets: " + triangles);
            sb.AppendLine("- Output folder: " + OutputRoot);
            sb.AppendLine();
            sb.AppendLine("## Prefabs");
            sb.AppendLine("| Source model | Role | Normalized bounds (m) | Triangles | Imported source |");
            sb.AppendLine("|---|---|---:|---:|---|");
            foreach (string row in rows) sb.AppendLine(row);
            if (missing.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("## Missing sources");
                foreach (string item in missing) sb.AppendLine("- " + item);
            }
            if (errors.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("## Build errors");
                foreach (string item in errors) sb.AppendLine("- " + item);
            }
            sb.AppendLine();
            sb.AppendLine("## Required review before runtime integration");
            sb.AppendLine("- Confirm silhouettes and proportions in the review scene.");
            sb.AppendLine("- Inspect material appearance under production URP lighting.");
            sb.AppendLine("- Confirm source pack license files are present in the imported pack.");
            sb.AppendLine("- Review triangles, renderer/material counts and texture memory on mobile.");
            sb.AppendLine("- Add colliders only to separate gameplay roots; visual prefabs intentionally have none.");
            sb.AppendLine("- No LODs are generated; assess hero trees and large props separately.");
            sb.AppendLine("- This manifest is a build record, not visual approval.");
            return sb.ToString();
        }

        private static string ToPascal(string source)
        {
            string[] parts = source.Split(new[] { '_', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Concat(parts.Select(p => char.ToUpperInvariant(p[0]) + p.Substring(1)));
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace("\\", "/");
            string folder = Path.GetFileName(path);
            EnsureFolder(parent);
            if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(parent, folder);
        }
    }
}
#endif
