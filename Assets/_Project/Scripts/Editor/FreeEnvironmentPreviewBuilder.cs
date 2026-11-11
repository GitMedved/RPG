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
    /// Creates a separate, disposable review scene for imported environment source models.
    /// It never edits production scenes, source models, prefabs, or approval flags.
    /// </summary>
    public static class FreeEnvironmentPreviewBuilder
    {
        private const string ModelRoot = "Assets/_Project/Art/Environment/Runtime/Models";
        private const string SceneDirectory = "Assets/_Project/Scenes/ArtReview";
        private const float CellSpacing = 3.2f;
        private const float TargetModelSize = 1.65f;
        private const int Columns = 6;

        [MenuItem("Tools/Mia & Max/Assets/Build Free Environment Review Scene")]
        public static void Build()
        {
            if (!Directory.Exists(ModelRoot))
            {
                EditorUtility.DisplayDialog("Mia & Max Asset Review",
                    "The environment model folder does not exist yet. Download a free pack and let Unity finish importing it first.",
                    "OK");
                return;
            }

            string scenePath = GetNextScenePath();

            List<string> modelPaths = FindModelAssets();
            if (modelPaths.Count == 0)
            {
                EditorUtility.DisplayDialog("Mia & Max Asset Review",
                    "No importable FBX, OBJ, DAE, GLB or glTF model assets were found. Download a pack, wait for import, then run the inventory and try again.",
                    "OK");
                return;
            }

            Directory.CreateDirectory(SceneDirectory);
            Scene previousActiveScene = SceneManager.GetActiveScene();
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            scene.name = "FreeEnvironmentPreview";
            SceneManager.SetActiveScene(scene);

            CreateLighting();
            CreateGround(modelPaths.Count);

            int shown = 0;
            for (int i = 0; i < modelPaths.Count; i++)
            {
                GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(modelPaths[i]);
                if (source == null) continue;

                int row = shown / Columns;
                int column = shown % Columns;
                Vector3 cellCenter = new Vector3(
                    (column - (Columns - 1) * 0.5f) * CellSpacing,
                    0f,
                    -row * CellSpacing);

                GameObject instance = PrefabUtility.InstantiatePrefab(source, scene) as GameObject;
                if (instance == null)
                    instance = UnityEngine.Object.Instantiate(source);

                instance.name = "REVIEW_ONLY_" + Path.GetFileNameWithoutExtension(modelPaths[i]);
                NormalizeAndPlace(instance, cellCenter);
                CreateLabel(Path.GetFileNameWithoutExtension(modelPaths[i]), cellCenter + new Vector3(0f, 0.08f, -1.15f));
                shown++;
            }

            if (shown == 0)
            {
                EditorSceneManager.CloseScene(scene, true);
                if (previousActiveScene.IsValid() && previousActiveScene.isLoaded)
                    SceneManager.SetActiveScene(previousActiveScene);
                EditorUtility.DisplayDialog("Mia & Max Asset Review",
                    "Model files were found, but Unity could not load them as GameObjects. Check the Console and model importer settings.",
                    "OK");
                return;
            }

            CreateCamera(shown);
            EditorSceneManager.MarkSceneDirty(scene);
            bool saved = EditorSceneManager.SaveScene(scene, scenePath);
            if (previousActiveScene.IsValid() && previousActiveScene.isLoaded)
                SceneManager.SetActiveScene(previousActiveScene);
            AssetDatabase.Refresh();

            if (saved)
            {
                EditorUtility.DisplayDialog("Mia & Max Asset Review",
                    "Created a separate review scene with " + shown + " model previews:\n" + scenePath +
                    "\n\nThis scene is for visual inspection only. No production scene, prefab, source model or approval flag was changed.",
                    "OK");
                Debug.Log("[Mia & Max] Free environment review scene created. Preview models: " + shown + ". Scene: " + scenePath);
            }
            else
            {
                Debug.LogError("[Mia & Max] Failed to save free environment review scene: " + scenePath);
            }
        }

        private static string GetNextScenePath()
        {
            string firstPath = SceneDirectory + "/FreeEnvironmentPreview.unity";
            if (!File.Exists(firstPath))
                return firstPath;

            int version = 2;
            string candidate;
            do
            {
                candidate = SceneDirectory + "/FreeEnvironmentPreview_" + version.ToString("D2") + ".unity";
                version++;
            }
            while (File.Exists(candidate));

            return candidate;
        }

        private static List<string> FindModelAssets()
        {
            string[] files = Directory.GetFiles(ModelRoot, "*", SearchOption.AllDirectories);
            return files
                .Where(path =>
                {
                    string extension = Path.GetExtension(path).ToLowerInvariant();
                    return extension == ".fbx" || extension == ".obj" || extension == ".dae" ||
                           extension == ".glb" || extension == ".gltf";
                })
                .Select(path => path.Replace('\\', '/'))
                .Where(path => !path.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static void NormalizeAndPlace(GameObject instance, Vector3 cellCenter)
        {
            Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
            {
                instance.transform.position = cellCenter;
                return;
            }

            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            float largest = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            if (largest > 0.0001f)
                instance.transform.localScale *= TargetModelSize / largest;

            renderers = instance.GetComponentsInChildren<Renderer>(true);
            bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            instance.transform.position += cellCenter - new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
        }

        private static void CreateGround(int modelCount)
        {
            int rows = Mathf.Max(1, Mathf.CeilToInt(modelCount / (float)Columns));
            float centerZ = -Mathf.Max(0, rows - 1) * CellSpacing * 0.5f;
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "REVIEW_ONLY_Ground";
            ground.transform.position = new Vector3(0f, -0.03f, centerZ);
            ground.transform.localScale = new Vector3(Columns * 1.15f, 1f, Mathf.Max(8f, rows * 0.42f));
            ground.GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"))
            {
                name = "REVIEW_ONLY_GroundMaterial",
                color = new Color(0.34f, 0.39f, 0.31f)
            };
        }

        private static void CreateLighting()
        {
            GameObject lightObject = new GameObject("REVIEW_ONLY_KeyLight");
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.15f;
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            GameObject fillObject = new GameObject("REVIEW_ONLY_FillLight");
            Light fill = fillObject.AddComponent<Light>();
            fill.type = LightType.Directional;
            fill.intensity = 0.45f;
            fillObject.transform.rotation = Quaternion.Euler(25f, 145f, 0f);
        }

        private static void CreateLabel(string label, Vector3 position)
        {
            GameObject labelObject = new GameObject("REVIEW_ONLY_Label_" + label);
            labelObject.transform.position = position;
            TextMesh text = labelObject.AddComponent<TextMesh>();
            text.text = label;
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.fontSize = 32;
            text.characterSize = 0.055f;
            text.color = Color.white;
        }

        private static void CreateCamera(int modelCount)
        {
            int rows = Mathf.CeilToInt(modelCount / (float)Columns);
            float centerZ = -Mathf.Max(0, rows - 1) * CellSpacing * 0.5f;

            GameObject cameraObject = new GameObject("REVIEW_ONLY_Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.16f, 0.19f, 0.22f);
            camera.orthographic = true;
            camera.orthographicSize = Mathf.Max(7f, rows * CellSpacing * 0.62f);
            cameraObject.transform.position = new Vector3(0f, 13f, centerZ - 15f);
            cameraObject.transform.rotation = Quaternion.Euler(32f, 0f, 0f);
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = Mathf.Max(100f, rows * CellSpacing + 40f);
            camera.tag = "MainCamera";
        }
    }
}
#endif
