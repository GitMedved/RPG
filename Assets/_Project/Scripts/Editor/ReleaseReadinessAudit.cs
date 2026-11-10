#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Read-only release gate report. It checks project configuration and asset presence,
    /// but never claims a build, device test, legal review or store approval has happened.
    /// </summary>
    public static class ReleaseReadinessAudit
    {
        private sealed class Result
        {
            public string Name;
            public bool Ready;
            public string Detail;
            public bool Blocker;
        }

        [MenuItem("Tools/Mia & Max/Release Readiness Audit")]
        public static void Run()
        {
            var results = new List<Result>();
            int enabledScenes = 0;
            int invalidEnabledScenes = 0;
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene == null || !scene.enabled || string.IsNullOrWhiteSpace(scene.path))
                    continue;

                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path) != null)
                    enabledScenes++;
                else
                    invalidEnabledScenes++;
            }

            Add(results, "Saved build scenes", enabledScenes > 0 && invalidEnabledScenes == 0,
                enabledScenes + " valid enabled scene(s); " + invalidEnabledScenes +
                " enabled scene path(s) missing or invalid. A runtime-generated prototype is not a substitute for a saved startup scene.", true);
            bool buildSettingsCommitted = File.Exists("ProjectSettings/EditorBuildSettings.asset");
            Add(results, "Persisted Build Settings", buildSettingsCommitted,
                buildSettingsCommitted
                    ? "ProjectSettings/EditorBuildSettings.asset exists in the project."
                    : "ProjectSettings/EditorBuildSettings.asset is missing; configure and save the startup scene in Build Settings, then commit the file.", true);

            string androidId = PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android);
            string iosId = PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.iOS);
            Add(results, "Android application ID", IsConfiguredId(androidId), Display(androidId), true);
            Add(results, "iOS bundle ID", IsConfiguredId(iosId), Display(iosId), true);
            Add(results, "Company and product identity",
                !string.IsNullOrWhiteSpace(PlayerSettings.companyName) &&
                !string.IsNullOrWhiteSpace(PlayerSettings.productName) &&
                PlayerSettings.companyName != "DefaultCompany",
                PlayerSettings.companyName + " / " + PlayerSettings.productName, true);
            Add(results, "Release version", !string.IsNullOrWhiteSpace(PlayerSettings.bundleVersion) &&
                PlayerSettings.bundleVersion != "0.1",
                "Version: " + Display(PlayerSettings.bundleVersion), true);

            bool urpAssignmentsReady = CheckUrpAssignments(out string urpDetail);
            Add(results, "URP pipeline asset and assignments", urpAssignmentsReady, urpDetail, true);
            Add(results, "Production Mia model", AssetCount("Mia t:Model", "Assets/_Project/Art/Characters/Runtime") > 0,
                "FBX/model presence only; rig and likeness need visual review.", true);
            Add(results, "Production Max model", AssetCount("Max t:Model", "Assets/_Project/Art/Characters/Runtime") > 0,
                "FBX/model presence only; rig and likeness need visual review.", true);
            Add(results, "Runtime character prefabs", AssetCount("t:Prefab", "Assets/Resources/MiaMax/Characters") >= 2,
                AssetCount("t:Prefab", "Assets/Resources/MiaMax/Characters") + " prefab(s) found; both Mia and Max are required.", true);
            Add(results, "Character animation clips", 
                AssetCount("t:AnimationClip", "Assets/_Project/Art/Characters/Runtime") >= 10,
                AssetCount("t:AnimationClip", "Assets/_Project/Art/Characters/Runtime") + " clip(s) found across character runtime folders. Review required actions for both characters.", true);

            Add(results, "Generated surface materials",
                AssetCount("t:Material", "Assets/Resources/MiaMax/Materials") >= 40,
                AssetCount("t:Material", "Assets/Resources/MiaMax/Materials") + " / 40 expected materials.", true);
            Add(results, "Generated albedo maps",
                AssetCount("t:Texture2D", "Assets/_Project/Art/Textures/Generated/Albedo") >= 40,
                AssetCount("t:Texture2D", "Assets/_Project/Art/Textures/Generated/Albedo") + " / 40 expected maps.", true);
            Add(results, "Generated normal maps",
                AssetCount("t:Texture2D", "Assets/_Project/Art/Textures/Generated/Normal") >= 40,
                AssetCount("t:Texture2D", "Assets/_Project/Art/Textures/Generated/Normal") + " / 40 expected maps.", true);
            Add(results, "Generated height maps",
                AssetCount("t:Texture2D", "Assets/_Project/Art/Textures/Generated/Height") >= 40,
                AssetCount("t:Texture2D", "Assets/_Project/Art/Textures/Generated/Height") + " / 40 expected maps.", true);
            int uiSprites = AssetCount("t:Sprite", "Assets/Resources/MiaMax/UI");
            Add(results, "UI production sprites", uiSprites >= 10,
                uiSprites + " / 10 generated sprite(s) in Resources/MiaMax/UI. Review contrast, scaling and visual quality.", true);
            Add(results, "Voice recordings", AssetCount("t:AudioClip", "Assets/Resources/MiaMax/Audio/Voice") >= 16,
                AssetCount("t:AudioClip", "Assets/Resources/MiaMax/Audio/Voice") + " / 16 expected takes.", true);
            int soundEffects = AssetCount("t:AudioClip", "Assets/Resources/MiaMax/Audio/SFX");
            Add(results, "Sound effects", soundEffects >= 11,
                soundEffects + " / 11 expected gameplay sound effect(s). Review event coverage and mix.", true);
            int musicClips = AssetCount("t:AudioClip", "Assets/Resources/MiaMax/Audio/Music");
            int ambienceClips = AssetCount("t:AudioClip", "Assets/Resources/MiaMax/Audio/Ambience");
            Add(results, "Music and ambience", musicClips >= 3 && ambienceClips >= 4,
                musicClips + " music track(s) / 3 expected; " + ambienceClips +
                " ambience loop(s) / 4 expected. Verify seamless looping and mix on target devices.", true);

            bool smokeChecksPassed = SessionState.GetBool("MiaMax.CoreSmokeChecks.Passed", false);
            Add(results, "Core automated smoke checks", smokeChecksPassed,
                smokeChecksPassed
                    ? "16 / 16 deterministic domain/save checks passed in this Unity Editor session. Play Mode, scene interaction, UI, import and device checks remain separate."
                    : "Not passed in this Editor session. Run Tools → Mia & Max → Run Core Smoke Checks, then run this audit again. The result is session-scoped and is not portable evidence.", true);
            Add(results, "Privacy / child safety / store review", false,
                "Manual legal and platform-policy sign-off is required; no approval is inferred from project files.", true);
            Add(results, "Physical device validation", false,
                "Android and iOS builds must be installed and tested on real devices.", true);

            int ready = 0, blockers = 0;
            var report = new StringBuilder();
            report.AppendLine("MIA & MAX — RELEASE READINESS AUDIT");
            report.AppendLine("Read-only static checks. Presence/configuration is not proof of visual quality or device behaviour.");
            report.AppendLine();
            foreach (Result result in results)
            {
                if (result.Ready) ready++;
                else if (result.Blocker) blockers++;
                report.AppendLine((result.Ready ? "[READY] " : result.Blocker ? "[BLOCKER] " : "[PENDING] ") +
                    result.Name + " — " + result.Detail);
            }
            report.AppendLine();
            report.AppendLine("Static checks ready: " + ready + " / " + results.Count);
            report.AppendLine("Release blockers still open: " + blockers);
            report.AppendLine("A release candidate is not approved until every blocker is closed and Android/iOS build + device test evidence is recorded.");

            Debug.Log(report.ToString());
            EditorUtility.DisplayDialog("Mia & Max — Release Readiness",
                "Static checks ready: " + ready + " / " + results.Count +
                "\nOpen release blockers: " + blockers +
                "\n\nFull report is in the Console. This tool does not build or certify the game.", "OK");
        }

        private static void Add(List<Result> results, string name, bool ready, string detail, bool blocker)
        {
            results.Add(new Result { Name = name, Ready = ready, Detail = detail, Blocker = blocker });
        }

        private static int AssetCount(string filter, string folder)
        {
            if (!AssetDatabase.IsValidFolder(folder))
                return 0;

            string[] assets = AssetDatabase.FindAssets(filter, new[] { folder });
            return assets == null ? 0 : assets.Length;
        }

        private static bool CheckUrpAssignments(out string detail)
        {
            int urpAssetCount = AssetCount("t:UniversalRenderPipelineAsset", "Assets");
            RenderPipelineAsset graphicsAsset = GraphicsSettings.defaultRenderPipeline;
            bool graphicsUsesUrp = graphicsAsset is UniversalRenderPipelineAsset;
            int incompatibleQualityOverrides = 0;
            string[] qualityNames = QualitySettings.names;
            int originalQuality = QualitySettings.GetQualityLevel();

            try
            {
                for (int i = 0; i < qualityNames.Length; i++)
                {
                    QualitySettings.SetQualityLevel(i, false);
                    RenderPipelineAsset qualityAsset = QualitySettings.renderPipeline;
                    if (qualityAsset != null && !(qualityAsset is UniversalRenderPipelineAsset))
                        incompatibleQualityOverrides++;
                }
            }
            finally
            {
                if (qualityNames.Length > 0)
                    QualitySettings.SetQualityLevel(Mathf.Clamp(originalQuality, 0, qualityNames.Length - 1), false);
            }

            bool ready = urpAssetCount > 0 && graphicsUsesUrp && incompatibleQualityOverrides == 0;
            detail = urpAssetCount + " URP asset(s); Graphics default: " +
                (graphicsAsset == null ? "(none)" : graphicsAsset.name) +
                "; incompatible quality overrides: " + incompatibleQualityOverrides +
                " / " + qualityNames.Length +
                ". Quality levels without an override inherit the Graphics asset.";
            return ready;
        }

        private static bool IsConfiguredId(string value)
        {
            return !string.IsNullOrWhiteSpace(value) &&
                value.Contains(".") &&
                !value.StartsWith("com.DefaultCompany") &&
                !value.Contains("yourcompany") &&
                !value.Contains("placeholder");
        }

        private static string Display(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "(not configured)" : value;
        }
    }
}
#endif
