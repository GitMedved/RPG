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
            EditorApplication.delayCall += EnsureSceneExists;
        }

        [MenuItem("Mia & Max/Setup Backyard Prototype Scene")]
        private static void EnsureSceneExists()
        {
            if (!Directory.Exists("Assets/_Project/Scenes"))
                Directory.CreateDirectory("Assets/_Project/Scenes");

            if (!File.Exists(ScenePath))
            {
                Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                EditorSceneManager.SaveScene(scene, ScenePath);
                EditorSceneManager.CloseScene(scene, true);
                Debug.Log("Created Mia & Max Backyard prototype scene without replacing the currently open scene.");
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
