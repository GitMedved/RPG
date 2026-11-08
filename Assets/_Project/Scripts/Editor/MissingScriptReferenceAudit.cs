#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Finds missing MonoBehaviour references in every project scene and prefab.
    /// This tool is read-only: it never removes unknown components or saves scenes.
    /// </summary>
    public static class MissingScriptReferenceAudit
    {
        [MenuItem("Tools/Mia & Max/Audit Missing Script References")]
        public static void Run()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Mia & Max — Missing Script Audit",
                    "Stop Play Mode, wait until Unity returns to Edit Mode, then run this audit again. " +
                    "Opening project scenes for a complete audit is not supported during Play Mode.",
                    "OK");
                Debug.LogWarning("[Mia & Max] Missing script audit was not run: exit Play Mode first.");
                return;
            }

            int total = 0;
            int scenesScanned = 0;
            int prefabsScanned = 0;
            List<string> findings = new List<string>();
            List<string> scanErrors = new List<string>();
            HashSet<string> loadedScenePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Scan currently loaded scenes first, preserving their current in-memory state.
            for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
            {
                Scene scene = SceneManager.GetSceneAt(sceneIndex);
                if (!scene.IsValid() || !scene.isLoaded) continue;

                if (!string.IsNullOrEmpty(scene.path))
                    loadedScenePaths.Add(scene.path);

                ScanScene(scene, findings, ref total);
                scenesScanned++;
            }

            // Open each unopened scene additively, inspect it, then close only that scene.
            // Existing scenes are not saved, closed, or reloaded.
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });
            for (int i = 0; i < sceneGuids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
                if (string.IsNullOrEmpty(path) || loadedScenePaths.Contains(path)) continue;

                Scene openedScene = default(Scene);
                try
                {
                    openedScene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                    if (!openedScene.IsValid() || !openedScene.isLoaded)
                    {
                        scanErrors.Add("Could not open scene for audit: " + path);
                        continue;
                    }

                    ScanScene(openedScene, findings, ref total);
                    scenesScanned++;
                }
                catch (Exception exception)
                {
                    scanErrors.Add("Scene '" + path + "' could not be scanned: " + exception.Message);
                }
                finally
                {
                    if (openedScene.IsValid() && openedScene.isLoaded)
                    {
                        try
                        {
                            EditorSceneManager.CloseScene(openedScene, true);
                        }
                        catch (Exception exception)
                        {
                            scanErrors.Add("Could not close audit scene '" + path + "': " + exception.Message);
                        }
                    }
                }
            }

            // Prefab contents are loaded into an isolated preview scene and always unloaded.
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
            for (int i = 0; i < prefabGuids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
                GameObject prefab = null;
                try
                {
                    prefab = PrefabUtility.LoadPrefabContents(path);
                    if (prefab != null)
                    {
                        ScanHierarchy(prefab, "Prefab '" + path + "'", findings, ref total);
                        prefabsScanned++;
                    }
                    else
                    {
                        scanErrors.Add("Could not load prefab for audit: " + path);
                    }
                }
                catch (Exception exception)
                {
                    scanErrors.Add("Prefab '" + path + "' could not be scanned: " + exception.Message);
                }
                finally
                {
                    if (prefab != null)
                    {
                        try
                        {
                            PrefabUtility.UnloadPrefabContents(prefab);
                        }
                        catch (Exception exception)
                        {
                            scanErrors.Add("Could not unload audit prefab '" + path + "': " + exception.Message);
                        }
                    }
                }
            }

            string report = "[Mia & Max] Missing script reference audit\n" +
                "Scenes scanned: " + scenesScanned + " / " + sceneGuids.Length + " project scene(s)\n" +
                "Prefabs scanned: " + prefabsScanned + " / " + prefabGuids.Length + " project prefab(s)\n" +
                "Missing MonoBehaviour references: " + total + "\n" +
                (findings.Count == 0 ? "No missing references found in scanned scenes or prefabs." :
                    string.Join("\n", findings.ToArray())) +
                (scanErrors.Count == 0 ? "" : "\n\nScan warnings (" + scanErrors.Count + "):\n" +
                    string.Join("\n", scanErrors.ToArray())) +
                "\n\nRead-only audit: no components were removed and no scenes were saved. " +
                "Runtime-generated objects are not included.";

            if (total > 0)
                Debug.LogError(report);
            else if (scanErrors.Count > 0)
                Debug.LogWarning(report);
            else
                Debug.Log(report);

            EditorUtility.DisplayDialog("Mia & Max — Missing Script Audit",
                "Missing references: " + total +
                "\nScenes scanned: " + scenesScanned + " / " + sceneGuids.Length +
                "\nPrefabs scanned: " + prefabsScanned + " / " + prefabGuids.Length +
                "\nScan warnings: " + scanErrors.Count +
                "\n\nFull report is in the Console.", "OK");
        }

        private static void ScanScene(Scene scene, List<string> findings, ref int total)
        {
            GameObject[] roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
                ScanHierarchy(roots[i], "Scene '" + scene.path + "'", findings, ref total);
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
