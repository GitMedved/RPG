#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

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

            Add(results, "URP pipeline asset", AssetCount("t:RenderPipelineAsset", "Assets") > 0,
                AssetCount("t:RenderPipelineAsset", "Assets") + " pipeline asset(s) found. Confirm it is assigned in Graphics and every Quality tier.", true);
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
            Add(results, "UI production sprites", AssetCount("t:Sprite", "Assets/_Project/Art/UI") >= 10,
                AssetCount("t:Sprite", "Assets/_Project/Art/UI") + " sprite(s). Prototype UI is not release art.", true);
            Add(results, "Voice recordings", AssetCount("t:AudioClip", "Assets/Resources/MiaMax/Audio/Voice") >= 16,
                AssetCount("t:AudioClip", "Assets/Resources/MiaMax/Audio/Voice") + " / 16 expected takes.", true);
            Add(results, "Sound effects", AssetCount("t:AudioClip", "Assets/Resources/MiaMax/Audio/SFX") >= 8,
                AssetCount("t:AudioClip", "Assets/Resources/MiaMax/Audio/SFX") + " clip(s) found; gameplay feedback set required.", true);
            Add(results, "Music and ambience", AssetCount("t:AudioClip", "Assets/_Project/Audio") >= 3,
                AssetCount("t:AudioClip", "Assets/_Project/Audio") + " clip(s) found. Check music, backyard and woodland loops.", true);

            Add(results, "Core automated smoke checks", false,
                "Run Tools → Mia & Max → Run Core Smoke Checks and record the result; this audit cannot execute tests.", true);
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
