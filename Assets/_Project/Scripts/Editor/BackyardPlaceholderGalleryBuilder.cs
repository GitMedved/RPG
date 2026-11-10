#if UNITY_EDITOR
using System.IO;
using MiaMax.Gameplay;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Builds a separate, non-gameplay scene for visual review of backyard placeholder prefabs.
    /// The existing gameplay scene is not modified.
    /// </summary>
    public static class BackyardPlaceholderGalleryBuilder
    {
        private const string PrefabFolder = "Assets/_Project/Art/Environment/Runtime/Prefabs";
        private const string SceneFolder = "Assets/_Project/Art/Environment/Runtime/Scenes";
        private const string ScenePath = SceneFolder + "/BackyardPlaceholderGallery.unity";

        private static readonly string[] PrefabNames =
        {
            "HouseExterior", "PatioDeck", "FenceStraight", "FenceCorner", "FenceGate",
            "MainTree", "SwingSet", "Sandbox", "ShrubCluster", "FlowerBed",
            "FlowerPot", "GardenPlot", "Workshop", "AdventureGate", "PathSegment"
        };

        [MenuItem("Tools/Mia & Max/Backyard/Create Placeholder Gallery Scene")]
        public static void CreateGallery()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Mia & Max", "Exit Play Mode before creating the gallery.", "OK");
                return;
            }

            if (File.Exists(ScenePath) &&
                !EditorUtility.DisplayDialog("Mia & Max",
                    "The gallery scene already exists. Replace this generated preview scene?",
                    "Replace", "Cancel"))
                return;

            EnsureFolder("Assets/_Project/Art/Environment/Runtime");
            EnsureFolder(SceneFolder);

            Scene gallery = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            Scene previousActive = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(gallery);

            try
            {
                GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
                floor.name = "Gallery_Floor";
                floor.transform.position = new Vector3(0f, -0.12f, 0f);
                floor.transform.localScale = new Vector3(5f, 1f, 4f);

                GameObject sunObject = new GameObject("Gallery_Sun");
                Light sun = sunObject.AddComponent<Light>();
                sun.type = LightType.Directional;
                sun.intensity = 1.2f;
                sun.shadows = LightShadows.Soft;
                sunObject.transform.rotation = Quaternion.Euler(48f, -30f, 0f);

                GameObject cameraObject = new GameObject("Gallery_Camera");
                Camera camera = cameraObject.AddComponent<Camera>();
                cameraObject.tag = "MainCamera";
                camera.orthographic = true;
                camera.orthographicSize = 25f;
                camera.transform.position = new Vector3(0f, 29f, -39f);
                camera.transform.rotation = Quaternion.LookRotation(
                    new Vector3(0f, 0f, 0f) - camera.transform.position);
                camera.clearFlags = CameraClearFlags.Skybox;

                int placed = 0;
                for (int i = 0; i < PrefabNames.Length; i++)
                {
                    string prefabPath = PrefabFolder + "/" + PrefabNames[i] + ".prefab";
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                    if (prefab == null)
                    {
                        Debug.LogError("[Mia & Max] Gallery skipped missing prefab: " + prefabPath);
                        continue;
                    }

                    GameObject instance = PrefabUtility.InstantiatePrefab(prefab, gallery) as GameObject;
                    if (instance == null)
                    {
                        Debug.LogError("[Mia & Max] Could not instantiate prefab in gallery: " + prefabPath);
                        continue;
                    }

                    int column = i % 5;
                    int row = i / 5;
                    instance.name = PrefabNames[i] + "_GALLERY_PREVIEW";
                    instance.transform.position = new Vector3((column - 2) * 9f, 0f, 8f - row * 8f);
                    instance.transform.rotation = Quaternion.identity;
                    placed++;
                }

                RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
                RenderSettings.ambientLight = new Color(0.68f, 0.70f, 0.74f);

                EditorSceneManager.MarkSceneDirty(gallery);
                bool saved = EditorSceneManager.SaveScene(gallery, ScenePath);
                if (!saved)
                    throw new IOException("Unity did not confirm saving the gallery scene.");

                Debug.Log("[Mia & Max] Placeholder gallery created: " + placed + "/" +
                    PrefabNames.Length + " prefab instances. Scene: " + ScenePath +
                    ". Preview only; not a gameplay scene.");

                EditorUtility.DisplayDialog("Mia & Max — Placeholder Gallery",
                    "Gallery scene saved. Prefab instances placed: " + placed + "/" +
                    PrefabNames.Length + "\n\nOpen this scene from Project:\n" + ScenePath +
                    "\n\nThis is a visual review scene only; gameplay scenes were not changed.",
                    "OK");
            }
            finally
            {
                if (previousActive.IsValid())
                    SceneManager.SetActiveScene(previousActive);
                EditorSceneManager.CloseScene(gallery, true);
                AssetDatabase.Refresh();
            }
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace('\\', '/');
            string folder = Path.GetFileName(path);
            if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, folder);
        }
    }
}
#endif
