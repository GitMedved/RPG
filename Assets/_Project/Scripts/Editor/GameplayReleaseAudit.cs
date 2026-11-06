#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using MiaMax.Domain.Progression;
using UnityEditor;
using UnityEngine;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Static integration audit for the M64-M70 gameplay/content/character foundations.
    /// It checks repository assets only; it does not run gameplay or certify release readiness.
    /// </summary>
    public static class GameplayReleaseAudit
    {
        private const string Scripts = "Assets/_Project/Scripts";

        [MenuItem("Tools/Mia & Max/Gameplay & Character Integration Audit")]
        public static void Run()
        {
            var report = new System.Text.StringBuilder();
            int ready = 0;
            int pending = 0;
            report.AppendLine("MIA & MAX — GAMEPLAY & CHARACTER INTEGRATION AUDIT");
            report.AppendLine();

            string[] requiredScripts =
            {
                "Domain/GameSession.cs",
                "Domain/Farming/FarmingService.cs",
                "Domain/Production/ProductionService.cs",
                "Domain/Orders/OrderBoardService.cs",
                "Domain/Progression/QuestService.cs",
                "Domain/Progression/UpgradeService.cs",
                "Domain/Progression/BackyardContentCatalog.cs",
                "Gameplay/MobileUiComponents.cs",
                "Gameplay/MobileUiNavigationStack.cs",
                "Characters/CharacterAnimationBridge.cs",
                "Editor/CharacterAnimatorStateMachineBuilder.cs",
                "MiniGames/Match3/Match3Board.cs",
                "MiniGames/Fishing/FishingTimingSession.cs",
                "MiniGames/Runner/MaxRunSession.cs",
                "MiniGames/Shared/MiniGameRewardService.cs"
            };

            for (int i = 0; i < requiredScripts.Length; i++)
                AddCheck(report, Scripts + "/" + requiredScripts[i], File.Exists(Scripts + "/" + requiredScripts[i]),
                    ref ready, ref pending);

            string[] catalogGuids = AssetDatabase.FindAssets("t:BackyardContentCatalog");
            if (catalogGuids.Length == 0)
            {
                report.AppendLine("[PENDING] Author a BackyardContentCatalog asset and populate reviewed release content.");
                pending++;
            }
            else
            {
                for (int i = 0; i < catalogGuids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(catalogGuids[i]);
                    BackyardContentCatalog catalog = AssetDatabase.LoadAssetAtPath<BackyardContentCatalog>(path);
                    if (catalog == null)
                    {
                        report.AppendLine("[PENDING] Could not load content catalog: " + path);
                        pending++;
                        continue;
                    }

                    List<string> issues = catalog.Validate();
                    if (issues.Count == 0)
                    {
                        report.AppendLine("[READY] Content catalog structure: " + path +
                            " (" + catalog.Entries.Count + " entries)");
                        ready++;
                    }
                    else
                    {
                        report.AppendLine("[PENDING] Content catalog has " + issues.Count + " issue(s): " + path);
                        for (int j = 0; j < issues.Count; j++)
                            report.AppendLine("         - " + issues[j]);
                        pending++;
                    }
                }
            }

            CheckCharacterClips("Mia", new[] { "idle", "walk", "run", "interact", "celebrate", "garden", "reaction" },
                report, ref ready, ref pending);
            CheckCharacterClips("Max", new[] { "idle", "walk", "run", "interact", "celebrate", "jump", "sleep", "play", "reaction" },
                report, ref ready, ref pending);

            report.AppendLine();
            report.AppendLine("Source/assets ready: " + ready);
            report.AppendLine("Pending: " + pending);
            report.AppendLine("Manual gates: run Unity compile and Play Mode; complete a fresh-profile gameplay loop; verify save/reload, economy and quest rewards; inspect character rigs/clips; test portrait/landscape and physical devices.");
            report.AppendLine("This is a static presence/structure audit, not a gameplay test or release certification.");

            Debug.Log(report.ToString());
            EditorUtility.DisplayDialog("Mia & Max Integration Audit",
                "Audit complete. Ready: " + ready + "\nPending: " + pending +
                "\n\nSee the Console for details. Manual Unity and device gates remain required.", "OK");
        }

        private static void AddCheck(System.Text.StringBuilder report, string path, bool found,
            ref int ready, ref int pending)
        {
            report.AppendLine((found ? "[READY] " : "[PENDING] ") + path);
            if (found) ready++;
            else pending++;
        }

        private static void CheckCharacterClips(string character, string[] requiredKeywords,
            System.Text.StringBuilder report, ref int ready, ref int pending)
        {
            string folder = "Assets/_Project/Art/Characters/Runtime/" + character;
            string[] guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { folder });
            var names = new HashSet<string>();
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
                for (int j = 0; j < assets.Length; j++)
                {
                    AnimationClip clip = assets[j] as AnimationClip;
                    if (clip != null) names.Add(clip.name.ToLowerInvariant());
                }
            }

            var missing = new List<string>();
            foreach (string keyword in requiredKeywords)
            {
                bool found = false;
                foreach (string name in names)
                    if (name.Contains(keyword)) { found = true; break; }
                if (!found) missing.Add(keyword);
            }

            if (missing.Count == 0)
            {
                report.AppendLine("[READY] " + character + " clip name coverage (" + requiredKeywords.Length + " actions)");
                ready++;
            }
            else
            {
                report.AppendLine("[PENDING] " + character + " clip name coverage; missing: " +
                    string.Join(", ", missing.ToArray()));
                pending++;
            }
        }
    }
}
#endif
