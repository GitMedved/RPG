#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// One-entry-point workflow for regenerating production assets and running read-only audits.
    /// All generated output remains in dedicated folders; gameplay placeholders are preserved.
    /// </summary>
    public static class ReleaseCandidateWorkflow
    {
        [MenuItem("Tools/Mia & Max/Release/Apply Mobile App Identity")]
        public static void ApplyMobileAppIdentity()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Mia & Max Release",
                    "Exit Play Mode before changing application identity.", "OK");
                return;
            }

            const string applicationId = "com.aleksdevelop.miamax";
            PlayerSettings.companyName = "AleksDevelop";
            PlayerSettings.productName = "Mia & Max Adventures";
            PlayerSettings.bundleVersion = "1.0";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, applicationId);
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, applicationId);
            AssetDatabase.SaveAssets();

            Debug.Log("[Mia & Max Release] Applied company/product identity and shared Android/iOS ID: " + applicationId);
            EditorUtility.DisplayDialog("Mia & Max Release",
                "Company: AleksDevelop\nProduct: Mia & Max Adventures\nAndroid and iOS ID: " + applicationId +
                "\nVersion: 1.0\n\nRun the Release Readiness Audit to verify.", "OK");
        }

        [MenuItem("Tools/Mia & Max/Release/Generate All Production Asset Packs")]
        public static void GenerateAllProductionAssets()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Mia & Max Release",
                    "Exit Play Mode before generating production assets.", "OK");
                return;
            }

            Debug.Log("[Mia & Max Release] Starting production asset generation.");
            RunGeneration("Backyard production models", BackyardProductionModelGenerator.GenerateAll);
            RunGeneration("Original audio pack", ProceduralAudioAssetGenerator.GenerateAll);
            RunGeneration("Original UI sprite pack", ProceduralUiArtGenerator.GenerateAll);

            Debug.Log("[Mia & Max Release] All production asset generators have completed their own reports. " +
                "Review each report for errors before running the read-only audit sequence.");
            EditorUtility.DisplayDialog("Mia & Max Release",
                "Production asset generators have been invoked: environment, audio and UI. " +
                "Review the three generation reports, then run Run All Read-Only Release Audits.",
                "OK");
        }

        [MenuItem("Tools/Mia & Max/Release/Run All Read-Only Release Audits")]
        public static void RunAllReadOnlyAudits()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog("Mia & Max Release",
                    "Exit Play Mode before running the release audit sequence.", "OK");
                return;
            }

            RunAudit("Backyard production prefab audit", BackyardProductionPrefabAudit.Run);
            RunAudit("Production surface asset audit", ProductionAssetAudit.Run);
            RunAudit("Audio asset audit", AudioAssetAudit.Run);
            RunAudit("Voice asset audit", VoiceAssetAudit.Run);
            RunAudit("Character runtime prefab validation", CharacterAssetPipeline.ValidateRuntimePrefabs);
            RunAudit("Missing script references audit", MissingScriptReferenceAudit.RunAll);
            RunAudit("Release readiness audit", ReleaseReadinessAudit.Run);

            Debug.Log("[Mia & Max Release] Read-only audit sequence finished. Review all individual Console reports; " +
                "this sequence does not certify visual quality, builds, devices, legal compliance or store approval.");
            EditorUtility.DisplayDialog("Mia & Max Release",
                "All read-only audit commands have been invoked. Review each Console report and its blockers. " +
                "No build, device test, legal review or store approval is inferred.",
                "OK");
        }

        private static void RunGeneration(string label, Action generation)
        {
            try
            {
                Debug.Log("[Mia & Max Release] Generating: " + label);
                generation();
            }
            catch (Exception exception)
            {
                Debug.LogError("[Mia & Max Release] Generation failed: " + label);
                Debug.LogException(exception);
            }
        }

        private static void RunAudit(string label, Action audit)
        {
            try
            {
                Debug.Log("[Mia & Max Release] Running: " + label);
                audit();
            }
            catch (Exception exception)
            {
                Debug.LogError("[Mia & Max Release] Audit failed: " + label);
                Debug.LogException(exception);
            }
        }
    }
}
#endif
