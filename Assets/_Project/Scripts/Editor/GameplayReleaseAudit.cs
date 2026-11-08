#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using MiaMax.Domain.Progression;
using UnityEditor;
using UnityEngine;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Static integration audit for the M64-M78 gameplay/content/character foundations.
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
                "Domain/Inventory/InventoryService.cs",
                "Domain/Farming/FarmingService.cs",
                "Domain/Production/ProductionService.cs",
                "Gameplay/FarmPlotPrototype.cs",
                "Gameplay/ProductionStationPrototype.cs",
                "Gameplay/BackyardFeedbackVfx.cs",
                "Gameplay/MobileHudCanvas.cs",
                "Gameplay/PrototypeHud.cs",
                "Domain/Production/ProductionService.cs",
                "Domain/Orders/OrderBoardService.cs",
                "Domain/Progression/UpgradeService.cs",
                "Domain/Progression/EconomyBalance.cs",
                "Domain/Progression/BackyardContentCatalog.cs",
                "Domain/Progression/QuestService.cs",
                "Editor/BackyardContentCatalogBuilder.cs",
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

            CheckRewardTransactionContract(report, ref ready, ref pending);
            CheckRuntimeCatalogIntegration(report, ref ready, ref pending);
            CheckCatalogDrivenLoop(report, ref ready, ref pending);
            CheckQuestPrerequisiteContract(report, ref ready, ref pending);
            CheckAtomicItemRewardContract(report, ref ready, ref pending);
            CheckFirstLoopSmokeCoverage(report, ref ready, ref pending);
            CheckInventoryFacade(report, ref ready, ref pending);
            CheckSharedProgressionRewards(report, ref ready, ref pending);
            CheckAtomicUpgradePurchase(report, ref ready, ref pending);
            CheckEconomyBalance(report, ref ready, ref pending);
            CheckInteractiveProgressPresentation(report, ref ready, ref pending);
            CheckActionFeedbackVfx(report, ref ready, ref pending);

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
                    CheckStarterCatalogCoverage(catalog, path, report, ref ready, ref pending);
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

        private static void CheckStarterCatalogCoverage(BackyardContentCatalog catalog, string path,
            System.Text.StringBuilder report, ref int ready, ref int pending)
        {
            string[] requiredIds =
            {
                "crop_carrot", "crop_tomato", "crop_strawberry",
                "recipe_carrot_soup", "recipe_strawberry_juice", "recipe_wooden_toy",
                "order_soup_01", "order_juice_01", "order_toy_01",
                "quest_first_steps", "quest_first_order", "quest_order_master", "quest_flower_friend",
                "match3", "fishing", "max_run"
            };
            BackyardContentKind[] requiredKinds =
            {
                BackyardContentKind.Crop, BackyardContentKind.Crop, BackyardContentKind.Crop,
                BackyardContentKind.Recipe, BackyardContentKind.Recipe, BackyardContentKind.Recipe,
                BackyardContentKind.Order, BackyardContentKind.Order, BackyardContentKind.Order,
                BackyardContentKind.StoryGoal, BackyardContentKind.StoryGoal,
                BackyardContentKind.StoryGoal, BackyardContentKind.StoryGoal,
                BackyardContentKind.MiniGame, BackyardContentKind.MiniGame, BackyardContentKind.MiniGame
            };
            var issues = new List<string>();
            for (int i = 0; i < requiredIds.Length; i++)
            {
                if (!catalog.TryGet(requiredIds[i], out BackyardContentEntry entry))
                    issues.Add(requiredIds[i] + " (missing)");
                else if (entry.Kind != requiredKinds[i])
                    issues.Add(requiredIds[i] + " (expected " + requiredKinds[i] + ", found " + entry.Kind + ")");
            }

            if (issues.Count == 0)
            {
                report.AppendLine("[READY] Starter gameplay catalog coverage: " +
                    requiredIds.Length + " IDs with correct content kinds (" + path + ")");
                ready++;
            }
            else
            {
                report.AppendLine("[PENDING] Starter gameplay catalog coverage issues (" + path + "): " +
                    string.Join(", ", issues.ToArray()));
                pending++;
            }
        }

        private static void AddCheck(System.Text.StringBuilder report, string path, bool found,
            ref int ready, ref int pending)
        {
            report.AppendLine((found ? "[READY] " : "[PENDING] ") + path);
            if (found) ready++;
            else pending++;
        }

        private static void CheckRewardTransactionContract(System.Text.StringBuilder report,
            ref int ready, ref int pending)
        {
            string sessionPath = Scripts + "/Domain/GameSession.cs";
            string questPath = Scripts + "/Domain/Progression/QuestService.cs";
            string savePath = Scripts + "/Infrastructure/Save/LocalSaveService.cs";
            bool sessionContract = File.Exists(sessionPath) &&
                File.ReadAllText(sessionPath).Contains("TryGrantRewards") &&
                File.ReadAllText(sessionPath).Contains("const int maxLevel = 100");
            bool questContract = File.Exists(questPath) &&
                File.ReadAllText(questPath).Contains("session.TryGrantRewards(rewardCoins, rewardExperience)");
            bool saveContract = File.Exists(savePath) &&
                File.ReadAllText(savePath).Contains("Math.Min(100, data.Progress.Level)");
            bool valid = sessionContract && questContract && saveContract;
            report.AppendLine((valid ? "[READY] " : "[PENDING] ") +
                "Atomic quest reward transaction and bounded level progression");
            if (valid) ready++; else pending++;
        }

        private static void CheckRuntimeCatalogIntegration(System.Text.StringBuilder report,
            ref int ready, ref int pending)
        {
            string questPath = Scripts + "/Domain/Progression/QuestService.cs";
            string builderPath = Scripts + "/Editor/BackyardContentCatalogBuilder.cs";
            bool questLoadsResources = File.Exists(questPath) &&
                File.ReadAllText(questPath).Contains("Resources.Load<BackyardContentCatalog>");
            bool builderUsesResources = File.Exists(builderPath) &&
                File.ReadAllText(builderPath).Contains("Assets/Resources/MiaMax/Content/BackyardContentCatalog.asset");
            bool valid = questLoadsResources && builderUsesResources;
            report.AppendLine((valid ? "[READY] " : "[PENDING] ") +
                "Runtime Resources catalog loading and starter catalog build path");
            if (valid) ready++; else pending++;
        }


        private static void CheckCatalogDrivenLoop(System.Text.StringBuilder report,
            ref int ready, ref int pending)
        {
            string productionPath = Scripts + "/Domain/Production/ProductionService.cs";
            string ordersPath = Scripts + "/Domain/Orders/OrderBoardService.cs";
            bool production = File.Exists(productionPath) &&
                File.ReadAllText(productionPath).Contains("BackyardContentKind.Recipe") &&
                File.ReadAllText(productionPath).Contains("TryStartProduction") &&
                File.ReadAllText(productionPath).Contains("TryCompleteProduction");
            bool orders = File.Exists(ordersPath) &&
                File.ReadAllText(ordersPath).Contains("BackyardContentKind.Order") &&
                File.ReadAllText(ordersPath).Contains("TryConsumeItemAndGrantRewards");
            bool valid = production && orders;
            report.AppendLine((valid ? "[READY] " : "[PENDING] ") +
                "Catalog-driven recipes, custom orders and atomic craft/delivery transactions");
            if (valid) ready++; else pending++;
        }

        private static void CheckQuestPrerequisiteContract(System.Text.StringBuilder report,
            ref int ready, ref int pending)
        {
            string catalogPath = Scripts + "/Domain/Progression/BackyardContentCatalog.cs";
            string questPath = Scripts + "/Domain/Progression/QuestService.cs";
            string builderPath = Scripts + "/Editor/BackyardContentCatalogBuilder.cs";
            bool catalog = File.Exists(catalogPath) &&
                File.ReadAllText(catalogPath).Contains("RequiredQuestId") &&
                File.ReadAllText(catalogPath).Contains("references missing story goal");
            bool quest = File.Exists(questPath) &&
                File.ReadAllText(questPath).Contains("private bool IsEligible") &&
                File.ReadAllText(questPath).Contains("IsAreaUnlocked");
            bool builder = File.Exists(builderPath) &&
                File.ReadAllText(builderPath).Contains("RequiredQuestId = \"quest_order_master\"");
            bool valid = catalog && quest && builder;
            report.AppendLine((valid ? "[READY] " : "[PENDING] ") +
                "Validated story prerequisites and ordered Flower Garden/Woodland Trail quest chain");
            if (valid) ready++; else pending++;
        }

        private static void CheckAtomicItemRewardContract(System.Text.StringBuilder report,
            ref int ready, ref int pending)
        {
            string sessionPath = Scripts + "/Domain/GameSession.cs";
            bool valid = File.Exists(sessionPath) &&
                File.ReadAllText(sessionPath).Contains("TryGrantItemAndRewards") &&
                File.ReadAllText(sessionPath).Contains("TryStartProduction") &&
                File.ReadAllText(sessionPath).Contains("TryCompleteProduction") &&
                File.ReadAllText(sessionPath).Contains("TryConsumeItemAndGrantRewards") &&
                File.ReadAllText(sessionPath).Contains("CanConsumeItemAndGrantRewards");
            report.AppendLine((valid ? "[READY] " : "[PENDING] ") +
                "Atomic inventory, coin and XP reward APIs");
            if (valid) ready++; else pending++;
        }

        private static void CheckFirstLoopSmokeCoverage(System.Text.StringBuilder report,
            ref int ready, ref int pending)
        {
            string smokePath = Scripts + "/Editor/EditorSmokeChecks.cs";
            bool valid = File.Exists(smokePath) &&
                File.ReadAllText(smokePath).Contains("Core smoke checks passed: \" + passed + \"/16.") &&
                File.ReadAllText(smokePath).Contains("TestFirstSessionLoop") &&
                File.ReadAllText(smokePath).Contains("TestCatalogDrivenProductionAndOrders");
            report.AppendLine((valid ? "[READY] " : "[PENDING] ") +
                "16-scenario smoke suite includes economy balance, atomic harvest and the fresh-profile loop");
            if (valid) ready++; else pending++;
        }


        private static void CheckInventoryFacade(System.Text.StringBuilder report,
            ref int ready, ref int pending)
        {
            string inventoryPath = Scripts + "/Domain/Inventory/InventoryService.cs";
            string productionPath = Scripts + "/Domain/Production/ProductionService.cs";
            string ordersPath = Scripts + "/Domain/Orders/OrderBoardService.cs";
            bool facade = File.Exists(inventoryPath) &&
                File.ReadAllText(inventoryPath).Contains("GetSnapshot()") &&
                File.ReadAllText(inventoryPath).Contains("new ItemStackData(stack.ItemId, stack.Quantity)");
            bool production = File.Exists(productionPath) &&
                File.ReadAllText(productionPath).Contains("inventory.Has(input, quantity)") &&
                File.ReadAllText(productionPath).Contains("public bool CanStart") &&
                File.ReadAllText(productionPath).Contains("public bool CanCollect");
            bool orders = File.Exists(ordersPath) &&
                File.ReadAllText(ordersPath).Contains("inventory.Has(order.ItemId, order.Quantity)");
            bool valid = facade && production && orders;
            report.AppendLine((valid ? "[READY] " : "[PENDING] ") +
                "Inventory facade, detached snapshots and production/order availability queries");
            if (valid) ready++; else pending++;
        }

        private static void CheckSharedProgressionRewards(System.Text.StringBuilder report,
            ref int ready, ref int pending)
        {
            string sessionPath = Scripts + "/Domain/GameSession.cs";
            string dailyPath = Scripts + "/Domain/Progression/DailyTaskService.cs";
            string achievementPath = Scripts + "/Domain/Progression/AchievementService.cs";
            bool shared = File.Exists(sessionPath) &&
                File.ReadAllText(sessionPath).Contains("private void ApplyExperience");
            bool daily = File.Exists(dailyPath) &&
                File.ReadAllText(dailyPath).Contains("session.TryGrantRewards(RewardCoins, RewardExperience)");
            bool achievement = File.Exists(achievementPath) &&
                File.ReadAllText(achievementPath).Contains("session.TryGrantRewards(definition.Coins, definition.Experience)");
            bool valid = shared && daily && achievement;
            report.AppendLine((valid ? "[READY] " : "[PENDING] ") +
                "Shared bounded XP progression for daily tasks, achievements and quests");
            if (valid) ready++; else pending++;
        }

        private static void CheckAtomicUpgradePurchase(System.Text.StringBuilder report,
            ref int ready, ref int pending)
        {
            string sessionPath = Scripts + "/Domain/GameSession.cs";
            string upgradePath = Scripts + "/Domain/Progression/UpgradeService.cs";
            bool session = File.Exists(sessionPath) &&
                File.ReadAllText(sessionPath).Contains("TryPurchaseUpgrade(int cost, bool gardenUpgrade, int maxLevel)");
            bool upgrade = File.Exists(upgradePath) &&
                File.ReadAllText(upgradePath).Contains("TryPurchaseUpgrade(GardenUpgradeCost") &&
                File.ReadAllText(upgradePath).Contains("TryPurchaseUpgrade(WorkshopUpgradeCost");
            bool valid = session && upgrade;
            report.AppendLine((valid ? "[READY] " : "[PENDING] ") +
                "Atomic upgrade currency deduction and level increment");
            if (valid) ready++; else pending++;
        }

        private static void CheckEconomyBalance(System.Text.StringBuilder report,
            ref int ready, ref int pending)
        {
            string economyPath = Scripts + "/Domain/Progression/EconomyBalance.cs";
            string upgradePath = Scripts + "/Domain/Progression/UpgradeService.cs";
            string farmingPath = Scripts + "/Domain/Farming/FarmingService.cs";
            bool economy = File.Exists(economyPath) &&
                File.ReadAllText(economyPath).Contains("ProductionDurationPercent") &&
                File.ReadAllText(economyPath).Contains("HarvestYield");
            bool upgrade = File.Exists(upgradePath) &&
                File.ReadAllText(upgradePath).Contains("CanUpgradeGarden") &&
                File.ReadAllText(upgradePath).Contains("NextGardenHarvestYield");
            bool farming = File.Exists(farmingPath) &&
                File.ReadAllText(farmingPath).Contains("TryGrantItemAndRewards(harvestedCrop");
            bool valid = economy && upgrade && farming;
            report.AppendLine((valid ? "[READY] " : "[PENDING] ") +
                "Central economy curves, upgrade affordability previews and atomic harvest rewards");
            if (valid) ready++; else pending++;
        }

        private static void CheckInteractiveProgressPresentation(System.Text.StringBuilder report,
            ref int ready, ref int pending)
        {
            string farmingPath = Scripts + "/Domain/Farming/FarmingService.cs";
            string farmVisualPath = Scripts + "/Gameplay/FarmPlotPrototype.cs";
            string productionPath = Scripts + "/Domain/Production/ProductionService.cs";
            string stationPath = Scripts + "/Gameplay/ProductionStationPrototype.cs";
            string hudPath = Scripts + "/Gameplay/PrototypeHud.cs";
            string canvasPath = Scripts + "/Gameplay/MobileHudCanvas.cs";
            string smokePath = Scripts + "/Editor/EditorSmokeChecks.cs";
            bool growth = File.Exists(farmingPath) &&
                File.ReadAllText(farmingPath).Contains("GrowthProgress01") &&
                File.ReadAllText(farmingPath).Contains("SecondsUntilHarvest");
            bool farmVisual = File.Exists(farmVisualPath) &&
                File.ReadAllText(farmVisualPath).Contains("Growing · ") &&
                File.ReadAllText(farmVisualPath).Contains("nextRefreshTime");
            bool production = File.Exists(productionPath) &&
                File.ReadAllText(productionPath).Contains("public float Progress01");
            bool station = File.Exists(stationPath) &&
                File.ReadAllText(stationPath).Contains("service.Progress01");
            bool upgradeUi = File.Exists(hudPath) &&
                File.ReadAllText(hudPath).Contains("CanUpgradeGarden") &&
                File.ReadAllText(hudPath).Contains("NextGardenHarvestYield") &&
                File.Exists(canvasPath) &&
                File.ReadAllText(canvasPath).Contains("journalActionButtons[7]");
            bool smoke = File.Exists(smokePath) &&
                File.ReadAllText(smokePath).Contains("GrowthProgress01") &&
                File.ReadAllText(smokePath).Contains("production.Progress01");
            bool valid = growth && farmVisual && production && station && upgradeUi && smoke;
            report.AppendLine((valid ? "[READY] " : "[PENDING] ") +
                "Crop/workshop progress presentation and mobile upgrade affordability states");
            if (valid) ready++; else pending++;
        }

        private static void CheckActionFeedbackVfx(System.Text.StringBuilder report,
            ref int ready, ref int pending)
        {
            string vfxPath = Scripts + "/Gameplay/BackyardFeedbackVfx.cs";
            string farmPath = Scripts + "/Gameplay/FarmPlotPrototype.cs";
            string stationPath = Scripts + "/Gameplay/ProductionStationPrototype.cs";
            bool vfx = File.Exists(vfxPath) &&
                File.ReadAllText(vfxPath).Contains("ParticleSystemStopAction.Destroy") &&
                File.ReadAllText(vfxPath).Contains("PlayWorkshopComplete");
            bool farm = File.Exists(farmPath) &&
                File.ReadAllText(farmPath).Contains("PlayPlant") &&
                File.ReadAllText(farmPath).Contains("PlayHarvest");
            bool station = File.Exists(stationPath) &&
                File.ReadAllText(stationPath).Contains("PlayWorkshopStart") &&
                File.ReadAllText(stationPath).Contains("PlayWorkshopProgress") &&
                File.ReadAllText(stationPath).Contains("PlayWorkshopComplete");
            bool valid = vfx && farm && station;
            report.AppendLine((valid ? "[READY] " : "[PENDING] ") +
                "One-shot planting, harvest and workshop particle feedback is integrated");
            if (valid) ready++; else pending++;
        }

        private static void CheckCharacterClips(string character, string[] requiredKeywords,
            System.Text.StringBuilder report, ref int ready, ref int pending)
        {
            string folder = "Assets/_Project/Art/Characters/Runtime/" + character;
            if (!AssetDatabase.IsValidFolder(folder))
            {
                report.AppendLine("[PENDING] " + character + " animation folder not created yet: " + folder);
                pending++;
                return;
            }
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
