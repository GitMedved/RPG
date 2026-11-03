#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiaMax.Gameplay.Editor
{
    /// <summary>Creates the initial empty Backyard scene once, then registers it in Build Settings.</summary>
    [InitializeOnLoad]
    public static class BackyardSceneSetup
    {
        private const string ScenePath = "Assets/_Project/Scenes/Backyard.unity";

        static BackyardSceneSetup()
        {
            // Do not interrupt editor startup with a save dialog or discard an untitled scene.
            EditorApplication.delayCall += () => EnsureSceneExists(false);
        }

        [MenuItem("Mia & Max/Setup Backyard Prototype Scene")]
        private static void SetupFromMenu()
        {
            EnsureSceneExists(true);
        }

        private static void EnsureSceneExists(bool userInitiated)
        {
            if (!Directory.Exists("Assets/_Project/Scenes"))
                Directory.CreateDirectory("Assets/_Project/Scenes");

            if (!File.Exists(ScenePath))
            {
                Scene activeScene = SceneManager.GetActiveScene();
                NewSceneMode mode = NewSceneMode.Additive;

                // Unity cannot create an additive scene while the current scene is untitled.
                // A clean, empty untitled startup scene can safely be replaced; otherwise ask
                // the user to save their work before creating the prototype scene.
                if (string.IsNullOrEmpty(activeScene.path))
                {
                    bool hasUnsavedWork = activeScene.isDirty || activeScene.rootCount > 0;
                    if (hasUnsavedWork)
                    {
                        if (!userInitiated)
                        {
                            Debug.LogWarning("Mia & Max: save the current untitled scene, then run Mia & Max > Setup Backyard Prototype Scene.");
                            return;
                        }

                        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                            return;

                        activeScene = SceneManager.GetActiveScene();
                        if (string.IsNullOrEmpty(activeScene.path))
                        {
                            Debug.LogWarning("Mia & Max: the current scene is still untitled. Save it before creating the Backyard scene.");
                            return;
                        }
                    }
                    else
                    {
                        mode = NewSceneMode.Single;
                    }
                }

                Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, mode);
                EditorSceneManager.SaveScene(scene, ScenePath);
                if (mode == NewSceneMode.Additive)
                    EditorSceneManager.CloseScene(scene, true);

                Debug.Log("Created Mia & Max Backyard prototype scene.");
            }

            EditorBuildSettingsScene[] current = EditorBuildSettings.scenes;
            foreach (EditorBuildSettingsScene entry in current)
                if (entry.path == ScenePath) return;

            var updated = new EditorBuildSettingsScene[current.Length + 1];
            current.CopyTo(updated, 0);
            updated[current.Length] = new EditorBuildSettingsScene(ScenePath, true);
            EditorBuildSettings.scenes = updated;
        }
    }
}
#endif
