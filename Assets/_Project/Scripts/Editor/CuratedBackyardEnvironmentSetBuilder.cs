#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Builds a review-only composition from selected Kenney models.
    /// It does not modify imported source models, gameplay prefabs, or production scenes.
    /// </summary>
    public static class CuratedBackyardEnvironmentSetBuilder
    {
        private const string ModelRoot = "Assets/_Project/Art/Environment/Runtime/Models/Kenney";
        private const string SceneRoot = "Assets/_Project/Scenes/ArtReview";
        private const float FenceScale = 1.0f;

        private sealed class Placement
        {
            public string Asset;
            public Vector3 Position;
            public Vector3 Euler;
            public float TargetHeight;
            public float TargetWidth;
        }

        [MenuItem("Tools/Mia & Max/Assets/Build Curated Backyard Environment Set")]
        public static void Build()
        {
            string[] guids = AssetDatabase.FindAssets("t:Model", new[] { ModelRoot });
            var paths = guids.Select(AssetDatabase.GUIDToAssetPath)
                .Where(p => !string.IsNullOrEmpty(p) && !p.Contains("/GLB format/"))
                .ToArray();

            var placements = CreateLayout();
            var resolved = new List<Tuple<Placement, string>>();
            var missing = new List<string>();

            foreach (Placement placement in placements)
            {
                string path = paths.FirstOrDefault(p =>
                    Path.GetFileNameWithoutExtension(p).Equals(placement.Asset, StringComparison.OrdinalIgnoreCase));
                if (string.IsNullOrEmpty(path) || AssetDatabase.LoadAssetAtPath<GameObject>(path) == null)
                    missing.Add(placement.Asset);
                else
                    resolved.Add(Tuple.Create(placement, path));
            }

            if (resolved.Count == 0)
            {
                EditorUtility.DisplayDialog("Curated Backyard Set",
                    "No selected Kenney models could be found. Check that the NatureKit and FurnitureKit models are imported.", "OK");
                return;
            }

            EnsureSceneFolder();
            Scene previous = SceneManager.GetActiveScene();
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            scene.name = "CuratedBackyardEnvironmentSet";
            SceneManager.SetActiveScene(scene);

            CreateLighting();
            CreateGround();
            GameObject root = new GameObject("MIA_MAX_CURATED_BACKYARD_ENVIRONMENT_REVIEW_ONLY");
            foreach (Tuple<Placement, string> item in resolved)
                AddModel(root.transform, item.Item1, item.Item2);

            CreateCamera();
            EditorSceneManager.MarkSceneDirty(scene);
            string scenePath = NextScenePath();
            bool saved = EditorSceneManager.SaveScene(scene, scenePath);
            if (previous.IsValid() && previous.isLoaded) SceneManager.SetActiveScene(previous);
            AssetDatabase.Refresh();

            string report = "[Mia & Max] CURATED BACKYARD ENVIRONMENT SET\n" +
                "Placed models: " + resolved.Count + "\n" +
                "Scene: " + scenePath + "\n" +
                "Selected: " + string.Join(", ", resolved.Select(x => x.Item1.Asset).Distinct()) +
                (missing.Count > 0 ? "\nMissing: " + string.Join(", ", missing.Distinct()) : "") +
                "\nReview-only composition. Original imported models, production prefabs, and gameplay scenes were not modified.";
            if (saved)
            {
                Debug.Log(report);
                EditorUtility.DisplayDialog("Curated Backyard Set",
                    "Created a separate composition with " + resolved.Count + " placed model instances.\n\n" +
                    scenePath + (missing.Count > 0 ? "\n\nMissing models: " + string.Join(", ", missing.Distinct()) : "") +
                    "\n\nThis is a review scene, not production integration.", "OK");
            }
            else Debug.LogError("[Mia & Max] Could not save curated backyard scene: " + scenePath);
        }

        private static List<Placement> CreateLayout()
        {
            var p = new List<Placement>();

            // Back boundary: modular fence sections and a gate, kept separate from gameplay collision.
            for (int i = 0; i < 7; i++)
                p.Add(new Placement { Asset = "fence_planks", Position = new Vector3(-9f + i * 3f, 0f, -8f), TargetWidth = 2.9f, TargetHeight = 1.45f });
            p.Add(new Placement { Asset = "fence_gate", Position = new Vector3(0f, 0f, -8f), TargetWidth = 2.1f, TargetHeight = 1.8f });

            // Side accents.
            p.Add(new Placement { Asset = "fence_corner", Position = new Vector3(-10.5f, 0f, -5.5f), Euler = new Vector3(0f, 90f, 0f), TargetWidth = 1.6f, TargetHeight = 1.5f });
            p.Add(new Placement { Asset = "fence_corner", Position = new Vector3(10.5f, 0f, -5.5f), Euler = new Vector3(0f, 180f, 0f), TargetWidth = 1.6f, TargetHeight = 1.5f });

            // Two main trees frame the sunny backyard.
            p.Add(new Placement { Asset = "tree_blocks", Position = new Vector3(-7.5f, 0f, -3.5f), TargetHeight = 5.0f });
            p.Add(new Placement { Asset = "tree_cone", Position = new Vector3(7.5f, 0f, -3.5f), TargetHeight = 4.6f });

            // A small seating nook.
            p.Add(new Placement { Asset = "bench", Position = new Vector3(0f, 0f, 3.6f), Euler = new Vector3(0f, 180f, 0f), TargetWidth = 2.0f, TargetHeight = 0.95f });
            p.Add(new Placement { Asset = "chairRounded", Position = new Vector3(-2.2f, 0f, 3.8f), Euler = new Vector3(0f, 25f, 0f), TargetWidth = 1.0f, TargetHeight = 0.95f });
            p.Add(new Placement { Asset = "chairCushion", Position = new Vector3(2.2f, 0f, 3.8f), Euler = new Vector3(0f, -25f, 0f), TargetWidth = 1.0f, TargetHeight = 0.95f });

            // Flower beds and low greenery, arranged in repeated clusters.
            string[] flowers = { "flower_purpleA", "flower_purpleB", "flower_purpleC", "plantSmall1", "plantSmall2", "plantSmall3" };
            for (int i = 0; i < flowers.Length; i++)
            {
                float x = -5.2f + i * 2.08f;
                p.Add(new Placement { Asset = flowers[i], Position = new Vector3(x, 0f, -1.1f), TargetHeight = 0.85f, TargetWidth = 0.85f });
                p.Add(new Placement { Asset = flowers[i], Position = new Vector3(x, 0f, 1.0f), TargetHeight = 0.72f, TargetWidth = 0.72f });
            }

            // Natural stone accents at the edges, not a fabricated path.
            p.Add(new Placement { Asset = "cliff_blockHalf_stone", Position = new Vector3(-8.5f, 0f, 1.8f), TargetWidth = 1.5f, TargetHeight = 0.7f });
            p.Add(new Placement { Asset = "cliff_blockDiagonal_rock", Position = new Vector3(-7.2f, 0f, 2.1f), Euler = new Vector3(0f, 25f, 0f), TargetWidth = 1.3f, TargetHeight = 0.8f });
            p.Add(new Placement { Asset = "cliff_blockHalf_rock", Position = new Vector3(8.0f, 0f, 1.7f), TargetWidth = 1.5f, TargetHeight = 0.7f });
            p.Add(new Placement { Asset = "cliff_blockDiagonal_stone", Position = new Vector3(6.8f, 0f, 2.2f), Euler = new Vector3(0f, -25f, 0f), TargetWidth = 1.3f, TargetHeight = 0.8f });

            return p;
        }

        private static void AddModel(Transform parent, Placement placement, string path)
        {
            GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            GameObject instance = PrefabUtility.InstantiatePrefab(source) as GameObject;
            if (instance == null) instance = UnityEngine.Object.Instantiate(source);
            instance.name = "REVIEW_ONLY_" + placement.Asset;
            instance.transform.SetParent(parent, false);
            instance.transform.localPosition = placement.Position;
            instance.transform.localRotation = Quaternion.Euler(placement.Euler);

            Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0) return;

            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);

            float scale = 1f;
            if (placement.TargetHeight > 0f && bounds.size.y > 0.001f)
                scale = placement.TargetHeight / bounds.size.y;
            if (placement.TargetWidth > 0f)
            {
                float horizontal = Mathf.Max(bounds.size.x, bounds.size.z);
                if (horizontal > 0.001f) scale = placement.TargetWidth / horizontal;
            }
            instance.transform.localScale = Vector3.one * Mathf.Clamp(scale, 0.05f, 10f);

            // Re-evaluate after scaling and place the model base on the review ground.
            bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);
            instance.transform.position += Vector3.up * (-bounds.min.y);
        }

        private static void CreateGround()
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "REVIEW_ONLY_BackyardGround";
            ground.transform.position = new Vector3(0f, -0.06f, 0f);
            ground.transform.localScale = new Vector3(2.5f, 1f, 1.6f);
            Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.name = "REVIEW_ONLY_GroundMaterial";
            material.SetColor("_BaseColor", new Color(0.31f, 0.48f, 0.27f));
            ground.GetComponent<Renderer>().sharedMaterial = material;
        }

        private static void CreateLighting()
        {
            GameObject sunObject = new GameObject("REVIEW_ONLY_Sun");
            Light sun = sunObject.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1.35f;
            sunObject.transform.rotation = Quaternion.Euler(48f, -32f, 0f);
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.68f, 0.72f, 0.68f);
        }

        private static void CreateCamera()
        {
            GameObject cameraObject = new GameObject("REVIEW_ONLY_Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.55f, 0.72f, 0.82f);
            camera.fieldOfView = 48f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 250f;
            cameraObject.transform.position = new Vector3(0f, 19f, 25f);
            cameraObject.transform.LookAt(new Vector3(0f, 1.4f, -1.5f));
        }

        private static void EnsureSceneFolder()
        {
            if (!AssetDatabase.IsValidFolder(SceneRoot))
                AssetDatabase.CreateFolder("Assets/_Project/Scenes", "ArtReview");
        }

        private static string NextScenePath()
        {
            string path = SceneRoot + "/CuratedBackyardEnvironmentSet.unity";
            if (!File.Exists(path)) return path;
            int n = 2;
            do { path = SceneRoot + "/CuratedBackyardEnvironmentSet_" + n.ToString("D2") + ".unity"; n++; }
            while (File.Exists(path));
            return path;
        }
    }
}
#endif
