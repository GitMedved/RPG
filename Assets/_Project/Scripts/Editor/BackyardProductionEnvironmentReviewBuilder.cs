#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Validates the visual-only environment prefabs and creates an isolated review scene.
    /// Does not modify gameplay scenes, source models, or production prefab assets.
    /// </summary>
    public static class BackyardProductionEnvironmentReviewBuilder
    {
        private const string PrefabRoot = "Assets/_Project/Art/Environment/Runtime/ProductionPrefabs/Environment";
        private const string SceneRoot = "Assets/_Project/Scenes/ArtReview";
        private const string[] AssetNames =
        {
            "FencePlanks", "FenceGate", "FenceCorner", "TreeBlocks", "TreeCone",
            "Bench", "ChairRounded", "ChairCushion",
            "FlowerPurpleA", "FlowerPurpleB", "FlowerPurpleC",
            "PlantSmall1", "PlantSmall2", "PlantSmall3",
            "CliffBlockHalfStone", "CliffBlockDiagonalRock",
            "CliffBlockHalfRock", "CliffBlockDiagonalStone"
        };

        private sealed class AuditRow
        {
            public string Name;
            public bool Valid;
            public string Detail;
            public int Renderers;
            public int Meshes;
            public int Triangles;
        }

        [MenuItem("Tools/Mia & Max/Assets/Validate & Review Production Environment Prefabs")]
        public static void Build()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Mia & Max", "Exit Play Mode before building the review scene.", "OK");
                return;
            }

            var rows = new List<AuditRow>();
            foreach (string name in AssetNames)
                rows.Add(Audit(name));

            int valid = rows.Count(r => r.Valid);
            int blocked = rows.Count - valid;
            string report = BuildReport(rows, valid, blocked);
            Debug.Log(report);

            if (valid == 0)
            {
                EditorUtility.DisplayDialog("Mia & Max — Environment Review",
                    "No valid production prefabs were found. See Console for details.", "OK");
                return;
            }

            string scenePath = CreateReviewScene(rows.Where(r => r.Valid).Select(r => r.Name).ToList());
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[Mia & Max] PRODUCTION ENVIRONMENT REVIEW SCENE\n" +
                "Valid prefabs: " + valid + " / " + AssetNames.Length +
                "\nBlocked prefabs: " + blocked +
                "\nScene: " + scenePath +
                "\nProduction prefabs and gameplay scenes were not modified.");

            EditorUtility.DisplayDialog("Mia & Max — Environment Review",
                "Valid prefabs: " + valid + " / " + AssetNames.Length +
                "\nBlocked: " + blocked +
                "\n\nReview scene created:\n" + scenePath +
                "\n\nOpen the scene from Project window. Inspect silhouettes, scale, grounding and materials. This is not visual approval.",
                "OK");
        }

        private static AuditRow Audit(string name)
        {
            var row = new AuditRow { Name = name, Valid = false };
            string path = PrefabRoot + "/ENV_" + name + ".prefab";
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (asset == null)
            {
                row.Detail = "Prefab asset missing";
                return row;
            }

            GameObject root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
                MeshFilter[] filters = root.GetComponentsInChildren<MeshFilter>(true);
                SkinnedMeshRenderer[] skinned = root.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                var issues = new List<string>();
                row.Renderers = renderers.Length;

                if (renderers.Length == 0) issues.Add("no Renderer components");

                foreach (MeshFilter filter in filters)
                {
                    if (filter.sharedMesh == null) issues.Add("MeshFilter has no mesh");
                    else
                    {
                        row.Meshes++;
                        row.Triangles += CountTriangles(filter.sharedMesh);
                    }
                }
                foreach (SkinnedMeshRenderer renderer in skinned)
                {
                    if (renderer.sharedMesh == null) issues.Add("SkinnedMeshRenderer has no mesh");
                    else
                    {
                        row.Meshes++;
                        row.Triangles += CountTriangles(renderer.sharedMesh);
                    }
                }
                if (row.Meshes == 0) issues.Add("no valid meshes");

                foreach (Renderer renderer in renderers)
                {
                    Material[] materials = renderer.sharedMaterials;
                    if (materials == null || materials.Length == 0)
                    {
                        issues.Add("renderer has no material slots");
                        continue;
                    }
                    if (materials.Any(m => m == null)) issues.Add("null material slot");
                }

                if (root.GetComponentsInChildren<Collider>(true).Length > 0)
                    issues.Add("visual prefab contains Collider");
                if (root.GetComponentsInChildren<Rigidbody>(true).Length > 0)
                    issues.Add("visual prefab contains Rigidbody");
                if (root.GetComponentsInChildren<MonoBehaviour>(true).Length > 0)
                    issues.Add("visual prefab contains MonoBehaviour");

                Bounds bounds = CalculateBounds(renderers);
                if (bounds.size.sqrMagnitude < 0.0001f) issues.Add("bounds are empty");
                else if (Mathf.Abs(bounds.min.y) > 0.08f)
                    issues.Add("grounding offset is " + bounds.min.y.ToString("0.000") + " m");

                row.Valid = issues.Count == 0;
                row.Detail = row.Valid
                    ? "OK"
                    : string.Join("; ", issues.Distinct().ToArray());
                return row;
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static string CreateReviewScene(List<string> validNames)
        {
            if (!AssetDatabase.IsValidFolder(SceneRoot))
                AssetDatabase.CreateFolder("Assets/_Project/Scenes", "ArtReview");

            Scene previous = SceneManager.GetActiveScene();
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            scene.name = "ProductionEnvironmentPrefabReview";

            CreateLighting();
            CreateGround(validNames.Count);

            GameObject root = new GameObject("MIA_MAX_PRODUCTION_ENVIRONMENT_REVIEW_ONLY");
            const int columns = 6;
            const float spacingX = 4.6f;
            const float spacingZ = 5.2f;
            int rows = Mathf.CeilToInt(validNames.Count / (float)columns);

            for (int i = 0; i < validNames.Count; i++)
            {
                string name = validNames[i];
                int col = i % columns;
                int row = i / columns;
                Vector3 position = new Vector3(
                    (col - (columns - 1) * 0.5f) * spacingX,
                    0f,
                    ((rows - 1) * 0.5f - row) * spacingZ);

                string prefabPath = PrefabRoot + "/ENV_" + name + ".prefab";
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                GameObject instance = PrefabUtility.InstantiatePrefab(prefab, scene) as GameObject;
                if (instance == null) instance = UnityEngine.Object.Instantiate(prefab);
                instance.name = "REVIEW_" + name;
                instance.transform.SetParent(root.transform, false);
                instance.transform.position = position;

                CreateLabel(root.transform, name, position + new Vector3(0f, 0.12f, -1.55f));
                CreatePlinth(root.transform, position + new Vector3(0f, -0.12f, 0f));
            }

            CreateCamera(validNames.Count, rows);
            EditorSceneManager.MarkSceneDirty(scene);
            string path = NextScenePath();
            bool saved = EditorSceneManager.SaveScene(scene, path);

            if (previous.IsValid() && previous.isLoaded)
                SceneManager.SetActiveScene(previous);

            if (!saved)
                throw new InvalidOperationException("Unity could not save review scene: " + path);
            return path;
        }

        private static void CreateGround(int count)
        {
            int rows = Mathf.CeilToInt(count / 6f);
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "REVIEW_ONLY_NeutralGround";
            ground.transform.position = new Vector3(0f, -0.04f, 0f);
            ground.transform.localScale = new Vector3(3.2f, 1f, Mathf.Max(1.8f, rows * 0.8f));
            Collider collider = ground.GetComponent<Collider>();
            if (collider != null) UnityEngine.Object.DestroyImmediate(collider);
            SetMaterial(ground.GetComponent<Renderer>(), "REVIEW_ONLY_Ground", new Color(0.42f, 0.48f, 0.39f));
        }

        private static void CreatePlinth(Transform parent, Vector3 position)
        {
            GameObject plinth = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            plinth.name = "REVIEW_ONLY_DisplayPlinth";
            plinth.transform.SetParent(parent, false);
            plinth.transform.position = position;
            plinth.transform.localScale = new Vector3(1.65f, 0.08f, 1.65f);
            Collider collider = plinth.GetComponent<Collider>();
            if (collider != null) UnityEngine.Object.DestroyImmediate(collider);
            SetMaterial(plinth.GetComponent<Renderer>(), "REVIEW_ONLY_Plinth", new Color(0.72f, 0.69f, 0.59f));
        }

        private static void CreateLabel(Transform parent, string label, Vector3 position)
        {
            GameObject go = new GameObject("REVIEW_ONLY_Label_" + label);
            go.transform.SetParent(parent, false);
            go.transform.position = position;
            go.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            TextMesh text = go.AddComponent<TextMesh>();
            text.text = label;
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.fontSize = 48;
            text.characterSize = 0.055f;
            text.color = new Color(0.12f, 0.12f, 0.12f);
        }

        private static void CreateLighting()
        {
            GameObject sunObject = new GameObject("REVIEW_ONLY_Sun");
            Light sun = sunObject.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1.25f;
            sunObject.transform.rotation = Quaternion.Euler(48f, -32f, 0f);
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.72f, 0.74f, 0.70f);
        }

        private static void CreateCamera(int count, int rows)
        {
            GameObject cameraObject = new GameObject("REVIEW_ONLY_Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.66f, 0.77f, 0.83f);
            camera.fieldOfView = 48f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 300f;
            float depth = Mathf.Max(1, rows) * 5.2f;
            camera.orthographic = true;
            camera.orthographicSize = Mathf.Max(10f, rows * 4.8f);
            cameraObject.transform.position = new Vector3(0f, Mathf.Max(18f, depth * 1.1f), Mathf.Max(22f, depth * 1.35f));
            cameraObject.transform.LookAt(new Vector3(0f, 1.0f, 0f));
        }

        private static void SetMaterial(Renderer renderer, string name, Color color)
        {
            if (renderer == null) return;
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) return;
            Material material = new Material(shader) { name = name };
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            renderer.sharedMaterial = material;
        }

        private static Bounds CalculateBounds(Renderer[] renderers)
        {
            if (renderers == null || renderers.Length == 0) return new Bounds(Vector3.zero, Vector3.zero);
            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);
            return bounds;
        }

        private static int CountTriangles(Mesh mesh)
        {
            int count = 0;
            for (int i = 0; i < mesh.subMeshCount; i++)
                if (mesh.GetTopology(i) == MeshTopology.Triangles)
                    count += (int)mesh.GetIndexCount(i) / 3;
            return count;
        }

        private static string BuildReport(List<AuditRow> rows, int valid, int blocked)
        {
            var sb = new StringBuilder();
            sb.AppendLine("[Mia & Max] PRODUCTION ENVIRONMENT PREFAB STRUCTURAL AUDIT");
            sb.AppendLine("Read-only audit. No source models, prefabs or gameplay scenes are modified.");
            sb.AppendLine("Structurally valid: " + valid + " / " + rows.Count);
            sb.AppendLine("Blocked: " + blocked);
            sb.AppendLine("Triangles across audited prefab assets: " + rows.Sum(r => r.Triangles));
            sb.AppendLine();
            foreach (AuditRow row in rows)
                sb.AppendLine((row.Valid ? "[VALID] " : "[BLOCKED] ") + row.Name +
                    " | renderers: " + row.Renderers +
                    " | meshes: " + row.Meshes +
                    " | triangles: " + row.Triangles +
                    " | " + row.Detail);
            sb.AppendLine();
            sb.AppendLine("This audit checks structure and grounding only. It does not approve art direction, texture quality, licensing, mobile performance or final gameplay placement.");
            return sb.ToString();
        }

        private static string NextScenePath()
        {
            string path = SceneRoot + "/ProductionEnvironmentPrefabReview.unity";
            if (!File.Exists(path)) return path;
            int version = 2;
            do
            {
                path = SceneRoot + "/ProductionEnvironmentPrefabReview_" + version.ToString("D2") + ".unity";
                version++;
            } while (File.Exists(path));
            return path;
        }
    }
}
#endif
