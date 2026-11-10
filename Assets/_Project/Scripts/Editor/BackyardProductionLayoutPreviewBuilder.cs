#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Builds a separate backyard composition preview from the explicitly temporary
    /// production prefabs. It never modifies gameplay scenes or build settings.
    /// </summary>
    public static class BackyardProductionLayoutPreviewBuilder
    {
        private const string PrefabFolder = "Assets/_Project/Art/Environment/Runtime/ProductionPrefabs";
        private const string SceneFolder = "Assets/_Project/Art/Environment/Runtime/Scenes";
        private const string MaterialFolder = "Assets/_Project/Art/Environment/Runtime/Materials";
        private const string ScenePath = SceneFolder + "/BackyardProductionLayoutPreview.unity";

        private static readonly List<string> MissingAssets = new List<string>();
        private static int placed;

        [MenuItem("Tools/Mia & Max/Backyard/Create Production Yard Preview")]
        public static void CreatePreview()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Mia & Max", "Exit Play Mode before creating the preview.", "OK");
                return;
            }

            if (File.Exists(ScenePath) &&
                !EditorUtility.DisplayDialog("Mia & Max",
                    "The composed yard preview already exists. Replace this generated preview scene?",
                    "Replace", "Cancel"))
                return;

            EnsureFolder("Assets/_Project/Art/Environment/Runtime");
            EnsureFolder(SceneFolder);
            EnsureFolder(MaterialFolder);

            MissingAssets.Clear();
            placed = 0;
            Scene previousActive = SceneManager.GetActiveScene();
            Scene preview = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);

            try
            {
                SceneManager.SetActiveScene(preview);
                BuildGround();
                BuildHouseAndPatio();
                BuildPerimeter();
                BuildLandmarks();
                BuildGardenDetails();
                BuildLightingAndCamera();

                AssetDatabase.SaveAssets();
                EditorSceneManager.MarkSceneDirty(preview);
                if (!EditorSceneManager.SaveScene(preview, ScenePath))
                    throw new IOException("Unity did not confirm saving the composed yard preview.");

                string report = "[Mia & Max] Production backyard preview created\n" +
                    "Prefab instances placed: " + placed + "\n" +
                    "Missing prefabs: " + MissingAssets.Count + "\n" +
                    (MissingAssets.Count == 0 ? "All requested production prefabs were found." :
                        string.Join("\n", MissingAssets.ToArray())) +
                    "\nScene: " + ScenePath +
                    "\nPreview only. Placeholder geometry is not production art; gameplay scenes and build settings were not changed.";
                if (MissingAssets.Count == 0) Debug.LogWarning(report);
                else Debug.LogError(report);

                EditorUtility.DisplayDialog("Mia & Max — Production Yard Preview",
                    "Scene saved. Prefab instances: " + placed +
                    "\nMissing prefabs: " + MissingAssets.Count +
                    "\n\nOpen from Project:\n" + ScenePath +
                    "\n\nThis is a visual layout preview only. It does not replace the playable backyard or modify Build Settings.",
                    "OK");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorUtility.DisplayDialog("Mia & Max", "Preview creation failed. See Console for details.", "OK");
            }
            finally
            {
                if (previousActive.IsValid() && previousActive.isLoaded)
                    SceneManager.SetActiveScene(previousActive);
                if (preview.IsValid() && preview.isLoaded)
                    EditorSceneManager.CloseScene(preview, true);
                AssetDatabase.Refresh();
            }
        }

        private static void BuildGround()
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Preview_Ground";
            ground.transform.position = new Vector3(0f, -0.15f, 0f);
            ground.transform.localScale = new Vector3(3.8f, 1f, 3.2f);
            ground.GetComponent<Renderer>().sharedMaterial = MakeMaterial("Preview_Lawn", new Color(0.23f, 0.39f, 0.19f));
        }

        private static void BuildHouseAndPatio()
        {
            Place("HouseExterior", new Vector3(-6.0f, 0f, 10.0f), Quaternion.identity);
            Place("PatioDeck", new Vector3(-6.0f, 0f, 6.0f), Quaternion.identity);
            Place("Workshop", new Vector3(0.0f, 0f, 6.0f), Quaternion.identity);
            Place("FlowerPot", new Vector3(-11.4f, 0f, 7.2f), Quaternion.identity, 0.85f);
            Place("FlowerPot", new Vector3(-0.7f, 0f, 8.1f), Quaternion.identity, 0.85f);
        }

        private static void BuildPerimeter()
        {
            // Rear and front runs. Leave a centered opening in the front run for the gate.
            for (int i = 0; i < 10; i++)
            {
                float x = -14.4f + i * 3.2f;
                Place("FenceStraight", new Vector3(x, 0f, 14f), Quaternion.identity);
                if (i < 4 || i > 5)
                    Place("FenceStraight", new Vector3(x, 0f, -14f), Quaternion.identity);
            }

            Place("FenceGate", new Vector3(0f, 0f, -14f), Quaternion.identity);

            // Side runs and corners.
            for (int i = 0; i < 8; i++)
            {
                float z = -11.2f + i * 3.2f;
                Place("FenceStraight", new Vector3(-16f, 0f, z), Quaternion.Euler(0f, 90f, 0f));
                Place("FenceStraight", new Vector3(16f, 0f, z), Quaternion.Euler(0f, 90f, 0f));
            }

            Place("FenceCorner", new Vector3(-16f, 0f, 14f), Quaternion.identity);
            Place("FenceCorner", new Vector3(16f, 0f, 14f), Quaternion.Euler(0f, 90f, 0f));
            Place("FenceCorner", new Vector3(-16f, 0f, -14f), Quaternion.Euler(0f, -90f, 0f));
            Place("FenceCorner", new Vector3(16f, 0f, -14f), Quaternion.Euler(0f, 180f, 0f));
        }

        private static void BuildLandmarks()
        {
            Place("MainTree", new Vector3(-1.5f, 0f, 1.5f), Quaternion.identity);
            Place("SwingSet", new Vector3(-12f, 0f, -5f), Quaternion.identity);
            Place("Sandbox", new Vector3(9f, 0f, -7f), Quaternion.identity);
            Place("AdventureGate", new Vector3(14.2f, 0f, 8.2f), Quaternion.Euler(0f, 90f, 0f));
            Place("PathSegment", new Vector3(-2.0f, 0f, 2.5f), Quaternion.identity, 1.2f);
            Place("PathSegment", new Vector3(-2.0f, 0f, -1.0f), Quaternion.identity, 1.2f);
        }

        private static void BuildGardenDetails()
        {
            Place("ShrubCluster", new Vector3(-13.2f, 0f, 10.2f), Quaternion.identity);
            Place("ShrubCluster", new Vector3(11.5f, 0f, 10.4f), Quaternion.identity);
            Place("ShrubCluster", new Vector3(-13.2f, 0f, -10.0f), Quaternion.identity);
            Place("FlowerBed", new Vector3(-5.5f, 0f, 11.0f), Quaternion.identity);
            Place("FlowerBed", new Vector3(6.2f, 0f, 11.0f), Quaternion.identity);
            Place("FlowerBed", new Vector3(13.4f, 0f, 3.2f), Quaternion.Euler(0f, 90f, 0f));
            Place("GardenPlot", new Vector3(4.2f, 0f, 1.0f), Quaternion.identity);
            Place("GardenPlot", new Vector3(6.4f, 0f, 1.0f), Quaternion.identity);
            Place("GardenPlot", new Vector3(8.6f, 0f, 1.0f), Quaternion.identity);
        }

        private static void BuildLightingAndCamera()
        {
            GameObject sunObject = new GameObject("Preview_Sun");
            Light sun = sunObject.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 0.95f;
            sun.shadows = LightShadows.Soft;
            sunObject.transform.rotation = Quaternion.Euler(48f, -30f, 0f);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.62f, 0.65f, 0.68f);

            GameObject cameraObject = new GameObject("Preview_Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
            camera.orthographic = true;
            camera.orthographicSize = 22f;
            camera.transform.position = new Vector3(0f, 25f, -34f);
            camera.transform.rotation = Quaternion.LookRotation(new Vector3(0f, 0f, 1f) -
                camera.transform.position);
            camera.clearFlags = CameraClearFlags.Skybox;
        }

        private static void Place(string prefabName, Vector3 position, Quaternion rotation, float scale = 1f)
        {
            string path = PrefabFolder + "/" + prefabName + ".prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                if (!MissingAssets.Contains(prefabName)) MissingAssets.Add(prefabName);
                return;
            }

            GameObject instance = PrefabUtility.InstantiatePrefab(prefab, SceneManager.GetActiveScene()) as GameObject;
            if (instance == null)
            {
                if (!MissingAssets.Contains(prefabName)) MissingAssets.Add(prefabName);
                return;
            }

            instance.name = prefabName + "_LAYOUT_PREVIEW";
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            instance.transform.localScale = Vector3.one * scale;
            placed++;
        }

        private static Material MakeMaterial(string name, Color color)
        {
            string path = MaterialFolder + "/" + name + ".mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material != null) return material;

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            material = new Material(shader) { name = name, color = color };
            AssetDatabase.CreateAsset(material, path);
            return material;
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
