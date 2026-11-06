#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Finds missing MonoBehaviour references in loaded scenes and project prefabs.
    /// This tool is intentionally read-only: it never removes unknown components.
    /// </summary>
    public static class MissingScriptReferenceAudit
    {
        [MenuItem("Tools/Mia & Max/Audit Missing Script References")]
        public static void Run()
        {
            int total = 0;
            List<string> findings = new List<string>();

            for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
            {
                Scene scene = SceneManager.GetSceneAt(sceneIndex);
                if (!scene.IsValid() || !scene.isLoaded) continue;
                GameObject[] roots = scene.GetRootGameObjects();
                for (int i = 0; i < roots.Length; i++)
                    ScanHierarchy(roots[i], "Scene '" + scene.name + "'", findings, ref total);
            }

            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
            for (int i = 0; i < prefabGuids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
                GameObject prefab = null;
                try
                {
                    prefab = PrefabUtility.LoadPrefabContents(path);
                    if (prefab != null)
                        ScanHierarchy(prefab, "Prefab '" + path + "'", findings, ref total);
                }
                finally
                {
                    if (prefab != null) PrefabUtility.UnloadPrefabContents(prefab);
                }
            }

            string report = "[Mia & Max] Missing script reference audit\n" +
                "Missing MonoBehaviour references: " + total + "\n" +
                (findings.Count == 0 ? "No missing references found in loaded scenes or prefabs." :
                    string.Join("\n", findings.ToArray())) +
                "\n\nRead-only audit: no components were removed. " +
                "Runtime-generated objects and unopened scenes are not included.";
            if (total == 0) Debug.Log(report);
            else Debug.LogError(report);
        }

        private static void ScanHierarchy(GameObject root, string source,
            List<string> findings, ref int total)
        {
            Transform[] transforms = root.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transforms.Length; i++)
            {
                GameObject current = transforms[i].gameObject;
                int missing = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(current);
                if (missing <= 0) continue;

                total += missing;
                string hierarchy = BuildHierarchyPath(current.transform);
                findings.Add(source + " / " + hierarchy + " — missing components: " + missing);
            }
        }

        private static string BuildHierarchyPath(Transform current)
        {
            string path = current.name;
            while (current.parent != null)
            {
                current = current.parent;
                path = current.name + "/" + path;
            }
            return path;
        }
    }
}
#endif
