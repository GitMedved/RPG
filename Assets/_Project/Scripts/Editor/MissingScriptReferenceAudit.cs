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
    /// Read-only missing MonoBehaviour audit with independently runnable scene and prefab passes.
    /// No missing components are removed and no scenes are saved.
    /// </summary>
    public static class MissingScriptReferenceAudit
    {
        private enum AuditScope { All, Scenes, Prefabs }

        private static string currentAuditTarget = "Audit initialization";
        private static readonly List<string> capturedUnityWarnings = new List<string>();

        private static void CaptureAuditWarning(string condition, string stackTrace, LogType type)
        {
            if (type != LogType.Warning || string.IsNullOrEmpty(condition) ||
                condition.IndexOf("The referenced script (Unknown) on this Behaviour is missing!",
                    StringComparison.OrdinalIgnoreCase) < 0)
                return;

            capturedUnityWarnings.Add(
                "During: " + currentAuditTarget + "\\nWarning: " + condition +
                (string.IsNullOrEmpty(stackTrace) ? "" : "\\nStack: " + stackTrace));
        }

        [MenuItem("Tools/Mia & Max/Audit Missing Script References")]
        public static void RunAll()
        {
            RunAudit(AuditScope.All);
        }

        [MenuItem("Tools/Mia & Max/Diagnostics/Missing Scripts — Scenes Only")]
        public static void RunScenesOnly()
        {
            RunAudit(AuditScope.Scenes);
        }

        [MenuItem("Tools/Mia & Max/Diagnostics/Missing Scripts — Prefabs Only")]
        public static void RunPrefabsOnly()
        {
            RunAudit(AuditScope.Prefabs);
        }

        private static void RunAudit(AuditScope scope)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog(
                    "Mia & Max — Missing Script Audit",
                    "Stop Play Mode, wait until Unity returns to Edit Mode, then run this audit again.",
                    "OK");
                Debug.LogWarning("[Mia & Max] Missing script audit was not run: exit Play Mode first.");
                return;
            }

            int total = 0;
            int scenesScanned = 0;
            int prefabsScanned = 0;
            int sceneCount = 0;
            int prefabCount = 0;
            var findings = new List<string>();
            var scanErrors = new List<string>();
            var loadedScenePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            capturedUnityWarnings.Clear();
            Application.logMessageReceived += CaptureAuditWarning;

            try
            {
                if (scope == AuditScope.All || scope == AuditScope.Scenes)
                    ScanProjectScenes(findings, scanErrors, loadedScenePaths, ref total,
                        ref scenesScanned, ref sceneCount);

                if (scope == AuditScope.All || scope == AuditScope.Prefabs)
                    ScanProjectPrefabs(findings, scanErrors, ref total,
                        ref prefabsScanned, ref prefabCount);
            }
            catch (Exception exception)
            {
                scanErrors.Add("Unexpected audit exception during '" + currentAuditTarget +
                    "': " + exception);
            }
            finally
            {
                Application.logMessageReceived -= CaptureAuditWarning;
                currentAuditTarget = "Audit finished";
            }

            string scopeLabel = scope == AuditScope.All ? "Scenes and prefabs" :
                scope == AuditScope.Scenes ? "Scenes only" : "Prefabs only";

            string report =
                "[Mia & Max] Missing script reference audit — " + scopeLabel + "\\n" +
                "Scenes scanned: " + scenesScanned + " / " + sceneCount + " project scene(s)\\n" +
                "Prefabs scanned: " + prefabsScanned + " / " + prefabCount + " project prefab(s)\\n" +
                "Missing MonoBehaviour references: " + total + "\\n" +
                (findings.Count == 0
                    ? "No missing references found in scanned assets."
                    : string.Join("\\n", findings.ToArray())) +
                (scanErrors.Count == 0 ? "" :
                    "\\n\\nScan errors (" + scanErrors.Count + "):\\n" +
                    string.Join("\\n", scanErrors.ToArray())) +
                (capturedUnityWarnings.Count == 0 ? "" :
                    "\\n\\nCaptured Unity missing-script warnings (" +
                    capturedUnityWarnings.Count + "):\\n" +
                    string.Join("\\n\\n", capturedUnityWarnings.ToArray())) +
                "\\n\\nRead-only audit: no components were removed and no scenes were saved. " +
                "Runtime-generated objects are not included.";

            if (total > 0 || scanErrors.Count > 0)
                Debug.LogWarning(report);
            else
                Debug.Log(report);

            EditorUtility.DisplayDialog(
                "Mia & Max — Missing Script Audit",
                "Scope: " + scopeLabel +
                "\\nMissing references: " + total +
                "\\nScenes scanned: " + scenesScanned + " / " + sceneCount +
                "\\nPrefabs scanned: " + prefabsScanned + " / " + prefabCount +
                "\\nScan errors: " + scanErrors.Count +
                "\\nCaptured Unity warnings: " + capturedUnityWarnings.Count +
                "\\n\\nFull report is in the Console.",
                "OK");
        }

        private static void ScanProjectScenes(
            List<string> findings,
            List<string> scanErrors,
            HashSet<string> loadedScenePaths,
            ref int total,
            ref int scenesScanned,
            ref int sceneCount)
        {
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });
            sceneCount = sceneGuids.Length;
            currentAuditTarget = "Enumerating loaded scenes";

            // Inspect loaded scenes in place. They are never closed, reloaded, or saved.
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (!scene.IsValid() || !scene.isLoaded)
                    continue;

                if (!string.IsNullOrEmpty(scene.path))
                    loadedScenePaths.Add(scene.path);

                currentAuditTarget = "Inspecting loaded scene: " +
                    (string.IsNullOrEmpty(scene.path) ? scene.name : scene.path);
                try
                {
                    ScanScene(scene, findings, ref total);
                    if (IsProjectScene(scene.path))
                        scenesScanned++;
                }
                catch (Exception exception)
                {
                    scanErrors.Add("Loaded scene '" + scene.path +
                        "' could not be scanned: " + exception.Message);
                }
            }

            // Open only project scenes that are not already loaded.
            for (int i = 0; i < sceneGuids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
                if (string.IsNullOrEmpty(path) || loadedScenePaths.Contains(path))
                    continue;

                Scene openedScene = default(Scene);
                try
                {
                    currentAuditTarget = "Opening project scene: " + path;
                    openedScene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                    if (!openedScene.IsValid() || !openedScene.isLoaded)
                    {
                        scanErrors.Add("Could not open scene for audit: " + path);
                        continue;
                    }

                    currentAuditTarget = "Inspecting project scene: " + path;
                    ScanScene(openedScene, findings, ref total);
                    scenesScanned++;
                }
                catch (Exception exception)
                {
                    scanErrors.Add("Scene '" + path +
                        "' could not be scanned: " + exception.Message);
                }
                finally
                {
                    if (openedScene.IsValid() && openedScene.isLoaded)
                    {
                        try
                        {
                            currentAuditTarget = "Closing audit scene: " + path;
                            EditorSceneManager.CloseScene(openedScene, true);
                        }
                        catch (Exception exception)
                        {
                            scanErrors.Add("Could not close audit scene '" + path +
                                "': " + exception.Message);
                        }
                    }
                }
            }
        }

        private static bool IsProjectScene(string path)
        {
            return !string.IsNullOrEmpty(path) &&
                path.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase);
        }

        private static void ScanProjectPrefabs(
            List<string> findings,
            List<string> scanErrors,
            ref int total,
            ref int prefabsScanned,
            ref int prefabCount)
        {
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
            prefabCount = prefabGuids.Length;

            for (int i = 0; i < prefabGuids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
                GameObject prefab = null;

                try
                {
                    currentAuditTarget = "Loading prefab contents: " + path;
                    prefab = PrefabUtility.LoadPrefabContents(path);
                    if (prefab == null)
                    {
                        scanErrors.Add("Could not load prefab for audit: " + path);
                        continue;
                    }

                    currentAuditTarget = "Inspecting prefab: " + path;
                    ScanHierarchy(prefab, "Prefab '" + path + "'", findings, ref total);
                    prefabsScanned++;
                }
                catch (Exception exception)
                {
                    scanErrors.Add("Prefab '" + path +
                        "' could not be scanned: " + exception.Message);
                }
                finally
                {
                    if (prefab != null)
                    {
                        try
                        {
                            currentAuditTarget = "Unloading prefab contents: " + path;
                            PrefabUtility.UnloadPrefabContents(prefab);
                        }
                        catch (Exception exception)
                        {
                            scanErrors.Add("Could not unload audit prefab '" + path +
                                "': " + exception.Message);
                        }
                    }
                }
            }
        }

        private static void ScanScene(Scene scene, List<string> findings, ref int total)
        {
            GameObject[] roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
                ScanHierarchy(roots[i], "Scene '" + scene.path + "'", findings, ref total);
        }

        private static void ScanHierarchy(
            GameObject root, string source, List<string> findings, ref int total)
        {
            Transform[] transforms = root.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transforms.Length; i++)
            {
                GameObject current = transforms[i].gameObject;
                int missing = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(current);
                if (missing <= 0)
                    continue;

                total += missing;
                findings.Add(source + " / " + BuildHierarchyPath(current.transform) +
                    " — missing components: " + missing);
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
