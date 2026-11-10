#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiaMax.EditorTools
{
    /// <summary>Builds an isolated review scene for imported Mia and Max runtime prefabs.</summary>
    public static class CharacterProductionReviewSceneBuilder
    {
        private const string SceneFolder = "Assets/_Project/Art/Characters/Runtime/Scenes";
        private const string ScenePath = SceneFolder + "/CharacterProductionReview.unity";
        private const string MiaPrefabPath = "Assets/Resources/MiaMax/Characters/Mia.prefab";
        private const string MaxPrefabPath = "Assets/Resources/MiaMax/Characters/Max.prefab";

        [MenuItem("Tools/Mia & Max/Characters/Create Character Production Review Scene")]
        public static void CreateReviewScene()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Mia & Max Characters",
                    "Exit Play Mode before creating the character review scene.", "OK");
                return;
            }

            Scene existingOpen = SceneManager.GetSceneByPath(ScenePath);
            if (existingOpen.IsValid() && existingOpen.isLoaded)
            {
                EditorUtility.DisplayDialog("Mia & Max Characters",
                    "The character review scene is already open. Close it before rebuilding.", "OK");
                return;
            }

            GameObject miaPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(MiaPrefabPath);
            GameObject maxPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(MaxPrefabPath);
            if (miaPrefab == null || maxPrefab == null)
            {
                EditorUtility.DisplayDialog("Mia & Max Characters",
                    "Both runtime prefabs are required. Mia: " + (miaPrefab != null) +
                    "; Max: " + (maxPrefab != null) +
                    ". Import and review both base FBX files first.", "OK");
                return;
            }

            if (File.Exists(ScenePath) &&
                !EditorUtility.DisplayDialog("Mia & Max Characters",
                    "Replace the generated character review scene?", "Replace", "Cancel"))
                return;

            EnsureFolder("Assets/_Project/Art/Characters");
            EnsureFolder("Assets/_Project/Art/Characters/Runtime");
            EnsureFolder(SceneFolder);

            Scene previousActive = SceneManager.GetActiveScene();
            Scene preview = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            try
            {
                SceneManager.SetActiveScene(preview);
                BuildStage();

                GameObject mia = PrefabUtility.InstantiatePrefab(miaPrefab, preview) as GameObject;
                GameObject max = PrefabUtility.InstantiatePrefab(maxPrefab, preview) as GameObject;
                if (mia == null || max == null)
                    throw new System.InvalidOperationException("Unity could not instantiate both runtime character prefabs.");

                mia.name = "Mia_Review_Instance";
                mia.transform.position = new Vector3(-1.2f, 0f, 0f);
                mia.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                max.name = "Max_Review_Instance";
                max.transform.position = new Vector3(1.2f, 0f, 0f);
                max.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

                AddLabel("MIA", new Vector3(-1.2f, 0.04f, -1.35f));
                AddLabel("MAX", new Vector3(1.2f, 0.04f, -1.35f));

                EditorSceneManager.MarkSceneDirty(preview);
                if (!EditorSceneManager.SaveScene(preview, ScenePath))
                    throw new IOException("Unity did not confirm saving the character review scene.");

                Debug.Log("[Mia & Max] Character production review scene created. " +
                    "Mia and Max runtime prefabs placed side by side. " +
                    "This is an isolated review scene; gameplay scenes and Build Settings were not changed.\nScene: " + ScenePath);
                EditorUtility.DisplayDialog("Mia & Max Characters",
                    "Character review scene saved. Open from Project:\n" + ScenePath +
                    "\n\nThis scene does not modify gameplay or Build Settings.", "OK");
            }
            catch (System.Exception exception)
            {
                Debug.LogException(exception);
                EditorUtility.DisplayDialog("Mia & Max Characters",
                    "Character review scene creation failed. See Console.", "OK");
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

        private static void BuildStage()
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Review_Floor";
            floor.transform.localScale = new Vector3(0.7f, 1f, 0.5f);
            Renderer floorRenderer = floor.GetComponent<Renderer>();
            const string floorMaterialPath = SceneFolder + "/CharacterReview_Floor.mat";
            Material floorMaterial = AssetDatabase.LoadAssetAtPath<Material>(floorMaterialPath);
            if (floorMaterial == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) shader = Shader.Find("Standard");
                if (shader != null)
                {
                    floorMaterial = new Material(shader);
                    floorMaterial.name = "CharacterReview_Floor";
                    floorMaterial.color = new Color(0.72f, 0.70f, 0.62f);
                    AssetDatabase.CreateAsset(floorMaterial, floorMaterialPath);
                }
            }
            if (floorMaterial != null) floorRenderer.sharedMaterial = floorMaterial;

            GameObject lightObject = new GameObject("Review_Key_Light");
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.15f;
            light.shadows = LightShadows.Soft;
            lightObject.transform.rotation = Quaternion.Euler(48f, -28f, 0f);

            GameObject fillObject = new GameObject("Review_Fill_Light");
            Light fill = fillObject.AddComponent<Light>();
            fill.type = LightType.Directional;
            fill.intensity = 0.35f;
            fill.shadows = LightShadows.None;
            fillObject.transform.rotation = Quaternion.Euler(20f, 145f, 0f);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.62f, 0.65f, 0.70f);

            GameObject cameraObject = new GameObject("Character_Review_Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
            camera.fieldOfView = 34f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 100f;
            camera.transform.position = new Vector3(0f, 2.0f, -8.2f);
            camera.transform.LookAt(new Vector3(0f, 0.95f, 0f));
        }

        private static void AddLabel(string value, Vector3 position)
        {
            GameObject labelObject = new GameObject(value + "_Label");
            labelObject.transform.position = position;
            labelObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            TextMesh label = labelObject.AddComponent<TextMesh>();
            label.text = value;
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.characterSize = 0.18f;
            label.fontSize = 64;
            label.color = new Color(0.18f, 0.22f, 0.20f);
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
