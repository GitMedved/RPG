#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Creates a project-owned URP asset when missing and assigns it to Graphics
    /// and every quality level. Existing URP assets are reused.
    /// </summary>
    public static class UrpProjectSetup
    {
        private const string SettingsFolder = "Assets/_Project/Settings";
        private const string PipelineAssetPath = SettingsFolder + "/MiaMax_UniversalRenderPipelineAsset.asset";

        [MenuItem("Tools/Mia & Max/Setup or Repair URP Pipeline")]
        public static void SetupOrRepair()
        {
            try
            {
                EnsureFolder(SettingsFolder);

                UniversalRenderPipelineAsset pipeline = FindExistingPipeline();
                if (pipeline == null)
                {
                    pipeline = UniversalRenderPipelineAsset.Create();
                    if (pipeline == null)
                        throw new InvalidOperationException("Unity could not create a Universal Render Pipeline Asset.");

                    ScriptableRendererData rendererData = pipeline.LoadBuiltinRendererData();
                    if (rendererData == null)
                    {
                        UnityEngine.Object.DestroyImmediate(pipeline);
                        throw new InvalidOperationException("URP could not initialize its built-in Universal Renderer. No project settings were changed.");
                    }

                    AssetDatabase.CreateAsset(pipeline, PipelineAssetPath);
                    if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(rendererData)))
                        AssetDatabase.AddObjectToAsset(rendererData, pipeline);

                    EditorUtility.SetDirty(pipeline);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                GraphicsSettings.defaultRenderPipeline = pipeline;

                int originalQuality = QualitySettings.GetQualityLevel();
                try
                {
                    string[] qualityNames = QualitySettings.names;
                    for (int i = 0; i < qualityNames.Length; i++)
                    {
                        QualitySettings.SetQualityLevel(i, false);
                        QualitySettings.renderPipeline = pipeline;
                    }
                }
                finally
                {
                    if (QualitySettings.names.Length > 0)
                        QualitySettings.SetQualityLevel(Mathf.Clamp(originalQuality, 0, QualitySettings.names.Length - 1), false);
                }

                EditorUtility.SetDirty(pipeline);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log("[Mia & Max] URP setup completed. Asset: " +
                    AssetDatabase.GetAssetPath(pipeline) +
                    ". Graphics and all quality levels now reference this asset. Verify the scene visually and inspect Console for shader/import warnings.");

                EditorUtility.DisplayDialog(
                    "Mia & Max — URP",
                    "URP asset is created/reused and assigned to Graphics and all quality levels.\n\n" +
                    "Asset: " + AssetDatabase.GetAssetPath(pipeline) +
                    "\n\nPlease check the Scene/Game views and Console. This setup does not certify visual quality or device performance.",
                    "OK");
            }
            catch (Exception exception)
            {
                Debug.LogError("[Mia & Max] URP setup failed: " + exception);
                EditorUtility.DisplayDialog("Mia & Max — URP setup failed",
                    exception.Message + "\n\nSee Console for details. No release validation is implied.", "OK");
            }
        }

        private static UniversalRenderPipelineAsset FindExistingPipeline()
        {
            string[] guids = AssetDatabase.FindAssets("t:UniversalRenderPipelineAsset", new[] { "Assets" });
            if (guids == null || guids.Length == 0)
                return null;

            RenderPipelineAsset current = GraphicsSettings.defaultRenderPipeline;
            if (current is UniversalRenderPipelineAsset currentUrp)
                return currentUrp;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                UniversalRenderPipelineAsset candidate = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(path);
                if (candidate != null)
                    return candidate;
            }

            return null;
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
                return;

            string parent = System.IO.Path.GetDirectoryName(folderPath)?.Replace("\\", "/");
            string folderName = System.IO.Path.GetFileName(folderPath);
            if (string.IsNullOrEmpty(parent) || !AssetDatabase.IsValidFolder(parent))
                throw new InvalidOperationException("Parent folder does not exist: " + parent);

            AssetDatabase.CreateFolder(parent, folderName);
        }
    }
}
#endif
