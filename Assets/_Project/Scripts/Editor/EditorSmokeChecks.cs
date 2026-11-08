#if UNITY_EDITOR
using System;
using System.IO;
using MiaMax.Domain;
using MiaMax.Domain.Farming;
using MiaMax.Domain.MiniGames;
using MiaMax.Domain.Orders;
using MiaMax.Domain.Production;
using MiaMax.Domain.Progression;
using MiaMax.Infrastructure.Save;
using UnityEditor;
using UnityEngine;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Fast, deterministic editor smoke checks for core domain and local-save behavior.
    /// This is not a replacement for Play Mode, device tests or the Unity Test Framework.
    /// </summary>
    public static class EditorSmokeChecks
    {
        [MenuItem("Tools/Mia & Max/Run Core Smoke Checks")]
        public static void Run()
        {
            int passed = 0;
            try
            {
                RunCheck("Session economy and inventory", TestSessionEconomy, ref passed);
                RunCheck("Garden/workshop upgrades", TestUpgrades, ref passed);
                RunCheck("Production, orders and daily progress", TestProductionAndOrders, ref passed);
                RunCheck("Daily goal reward idempotency", TestDailyReward, ref passed);
                RunCheck("Save validation and backup recovery", TestSaveRecovery, ref passed);
                RunCheck("Interrupted temporary-save recovery", TestInterruptedSaveRecovery, ref passed);
                RunCheck("Content catalog safe authoring", TestContentCatalogAuthoring, ref passed);
                RunCheck("Six-step tutorial gameplay flow", TestTutorialFlow, ref passed);
                RunCheck("Atomic quest rewards and level cap", TestQuestRewardAtomicity, ref passed);
                RunCheck("Runtime content catalog drives quest metadata", TestRuntimeQuestCatalog, ref passed);
                RunCheck("Catalog-driven production and custom orders", TestCatalogDrivenProductionAndOrders, ref passed);
                RunCheck("Quest prerequisites and world-area unlocks", TestQuestPrerequisitesAndAreas, ref passed);
                RunCheck("First-session production-to-delivery loop", TestFirstSessionLoop, ref passed);
                RunCheck("Inventory facade and detached snapshots", TestInventoryService, ref passed);
                RunCheck("Shared level cap and atomic progression rewards", TestSharedProgressionTransactions, ref passed);
                RunCheck("Central economy balance and atomic harvest", TestEconomyAndHarvest, ref passed);

                string message = "Core smoke checks passed: " + passed + "/16.\n\n" +
                    "This verifies deterministic domain/save paths only. Play Mode, scene interaction, " +
                    "UI layout, FBX import and Android/iOS builds still require separate validation.";
                Debug.Log("[Mia & Max] " + message);
                EditorUtility.DisplayDialog("Mia & Max — Smoke Checks", message, "OK");
            }
            catch (Exception exception)
            {
                Debug.LogError("[Mia & Max] Core smoke checks failed after " + passed + " check(s).");
                Debug.LogException(exception);
                EditorUtility.DisplayDialog("Mia & Max — Smoke Checks Failed",
                    "Failed after " + passed + " check(s).\n\n" + exception.Message +
                    "\n\nSee the Console for the full stack trace.", "OK");
            }
        }

        private static void RunCheck(string label, Action check, ref int passed)
        {
            check();
            passed++;
            Debug.Log("[Mia & Max] [PASS] " + label);
        }

        private static void TestSessionEconomy()
        {
            GameSession session = NewSession();
            Require(session.TryAddCoins(100), "Positive coin grant should succeed.");
            Require(!session.TryAddCoins(0), "Zero coin grant must be rejected.");
            Require(!session.TryAddCoins(-1), "Negative coin grant must be rejected.");
            Require(session.TrySpendCoins(40), "Affordable purchase should succeed.");
            Require(!session.TrySpendCoins(61), "Unaffordable purchase must be rejected.");
            Require(session.Data.Progress.Coins == 60, "Wallet balance should remain 60.");

            Require(session.AddItem("test_item", 3), "Initial inventory grant should succeed.");
            Require(!session.AddItem("test_item", 0), "Zero item grant must be rejected.");
            Require(session.Data.Inventory[0].Quantity == 3, "Inventory stack should contain three items.");
            Require(session.TryRemoveItem("test_item", 2), "Affordable item removal should succeed.");
            Require(!session.TryRemoveItem("test_item", 2), "Removing more than the stack must fail.");
            Require(session.Data.Inventory[0].Quantity == 1, "Inventory stack should contain one item.");
        }

        private static void TestUpgrades()
        {
            GameSession session = NewSession();
            Require(session.TryAddCoins(500), "Test wallet setup failed.");
            UpgradeService upgrades = new UpgradeService(session);

            Require(upgrades.TryUpgradeGarden(), "First garden upgrade should succeed.");
            Require(upgrades.TryUpgradeGarden(), "Second garden upgrade should succeed.");
            Require(!upgrades.TryUpgradeGarden(), "Garden must stop at max level.");
            Require(upgrades.GardenLevel == 2 && upgrades.HarvestYield == 4,
                "Garden level two should produce four crops.");

            Require(upgrades.TryUpgradeWorkshop(), "First workshop upgrade should succeed.");
            Require(upgrades.AdjustProductionDuration(180) == 153,
                "Workshop level one should reduce 180 seconds to 153.");
            Require(upgrades.AdjustProductionDuration(181) == 154,
                "Workshop level one should round 181 seconds up to 154 deterministically.");
            Require(session.Data.WorkshopLevel == 1,
                "First workshop upgrade should persist level one.");
            session.Data.WorkshopLevel = 2;
            Require(upgrades.AdjustProductionDuration(180) == 126,
                "Workshop level two should reduce 180 seconds by 30 percent.");
        }

        private static void TestEconomyAndHarvest()
        {
            Require(EconomyBalance.UpgradeCost(0) == 60 &&
                EconomyBalance.UpgradeCost(1) == 120 &&
                EconomyBalance.UpgradeCost(2) == int.MaxValue,
                "Upgrade cost curve should be centralized and stop at max level.");
            Require(EconomyBalance.HarvestYield(0) == 2 &&
                EconomyBalance.HarvestYield(1) == 3 &&
                EconomyBalance.HarvestYield(2) == 4,
                "Garden yield curve should add one crop per level.");
            Require(EconomyBalance.AdjustProductionDuration(180, 0) == 180 &&
                EconomyBalance.AdjustProductionDuration(180, 1) == 153 &&
                EconomyBalance.AdjustProductionDuration(180, 2) == 126,
                "Workshop production duration curve should be deterministic.");

            GameSession session = NewSession();
            Require(session.TryAddCoins(60), "Could not seed garden upgrade wallet.");
            UpgradeService upgrades = new UpgradeService(session);
            Require(upgrades.CanUpgradeGarden && upgrades.NextGardenHarvestYield == 3,
                "Upgrade UI query should expose affordability and next yield.");
            Require(upgrades.TryUpgradeGarden(), "Garden level one purchase should succeed.");

            FarmingService farming = new FarmingService(session);
            FarmPlotData plot = farming.GetPlot("smoke_harvest_plot");
            plot.CropId = FarmingService.Carrot;
            plot.PlantedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 120;
            int coinsBefore = session.Data.Progress.Coins;
            int xpBefore = session.Data.Progress.Experience;
            Require(farming.TryHarvest("smoke_harvest_plot", out string harvested) &&
                harvested == FarmingService.Carrot,
                "Mature crop should harvest successfully.");
            Require(session.Data.Inventory.Exists(x => x.ItemId == FarmingService.Carrot && x.Quantity == 3),
                "Garden level one should grant three crops.");
            Require(session.Data.Progress.Coins == coinsBefore + EconomyBalance.HarvestCoins &&
                session.Data.Progress.Experience == xpBefore + EconomyBalance.HarvestExperience,
                "Harvest should grant its configured coin and XP rewards.");
            Require(!plot.IsPlanted, "Successful harvest should clear the farm plot.");

            GameSession overflowSession = NewSession();
            FarmingService overflowFarming = new FarmingService(overflowSession);
            FarmPlotData overflowPlot = overflowFarming.GetPlot("overflow_harvest_plot");
            overflowPlot.CropId = FarmingService.Carrot;
            overflowPlot.PlantedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 120;
            overflowSession.Data.Progress.Coins = int.MaxValue;
            Require(!overflowFarming.TryHarvest("overflow_harvest_plot", out _),
                "Harvest must reject an overflowing reward before changing inventory.");
            Require(overflowPlot.IsPlanted &&
                !overflowSession.Data.Inventory.Exists(x => x.ItemId == FarmingService.Carrot),
                "Rejected harvest must preserve the crop and leave inventory unchanged.");
        }

        private static void TestProductionAndOrders()
        {
            GameSession session = NewSession();
            DailyTaskService daily = new DailyTaskService(session);
            Require(session.AddItem("crop_carrot", 3), "Could not seed production ingredients.");

            ProductionService production = new ProductionService(session);
            Require(production.CanStart(ProductionService.CarrotSoup),
                "Production availability should report sufficient ingredients and a free slot.");
            Require(production.TryStart(ProductionService.CarrotSoup), "Soup production should start.");
            Require(!production.CanStart(ProductionService.CarrotSoup),
                "Production availability should reject a second job while the slot is occupied.");
            Require(!production.CanCollect, "An unfinished production job should not be collectable.");
            Require(!production.TryStart(ProductionService.CarrotSoup),
                "A second production job must not start while the slot is occupied.");

            production.ActiveJob.StartedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 10;
            production.ActiveJob.DurationSeconds = 1;
            Require(production.CanCollect, "Finished production should expose a collectable state.");
            Require(production.TryCollect(), "Finished soup should be collectable.");
            Require(!production.CanCollect, "Collected production should no longer be collectable.");
            Require(!production.TryCollect(), "A collected production job must not pay out twice.");
            Require(session.Data.Inventory.Exists(x => x.ItemId == "product_carrot_soup" && x.Quantity == 1),
                "Collected soup should exist in inventory.");
            Require(daily.GetSummary().Contains("Craft 1/1"),
                "Collecting a product should advance the daily craft goal.");

            OrderBoardService orders = new OrderBoardService(session);
            Require(orders.TryComplete("order_soup_01"), "Ready soup order should complete.");
            Require(!orders.TryComplete("order_soup_01"), "A completed order must not pay out twice.");
            Require(daily.GetSummary().Contains("Deliver 1/1"),
                "Completing an order should advance the daily delivery goal.");
            Require(!orders.TryComplete("unknown_order"), "Unknown orders must be rejected.");
        }

        private static void TestDailyReward()
        {
            GameSession session = NewSession();
            DailyTaskService daily = new DailyTaskService(session);
            daily.RecordHarvest();
            daily.RecordHarvest();
            daily.RecordCraft();
            daily.RecordDelivery();
            daily.RecordExploration();

            Require(daily.IsComplete, "All four daily goals should be complete.");
            int beforeCoins = session.Data.Progress.Coins;
            int beforeExperience = session.Data.Progress.Experience;
            Require(daily.TryClaimReward(), "Completed daily reward should be claimable.");
            Require(!daily.TryClaimReward(), "Daily reward must be one-time.");
            Require(session.Data.Progress.Coins == beforeCoins + DailyTaskService.RewardCoins,
                "Daily reward should grant the configured coin amount.");
            Require(session.Data.Progress.Experience == beforeExperience + DailyTaskService.RewardExperience,
                "Daily reward should grant the configured XP amount.");
        }

        private static void TestInterruptedSaveRecovery()
        {
            string directory = Path.Combine(Path.GetTempPath(),
                "MiaMaxInterruptedSave_" + Guid.NewGuid().ToString("N"));
            try
            {
                LocalSaveService saveService = new LocalSaveService(directory);
                PlayerSaveData initial = new PlayerSaveData();
                initial.CreatedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                initial.Progress.Coins = 100;
                saveService.Save(initial);

                PlayerSaveData next = saveService.LoadOrCreate();
                next.Progress.Coins = 200;
                saveService.Save(next); // Leaves a known-good backup at 100 coins.

                string primary = Path.Combine(directory, "mia_max_save.json");
                string temporary = Path.Combine(directory, "mia_max_save.tmp");
                File.Delete(primary);

                PlayerSaveData interrupted = new PlayerSaveData();
                interrupted.CreatedAtUnix = initial.CreatedAtUnix;
                interrupted.UpdatedAtUnix = next.UpdatedAtUnix + 60;
                interrupted.Progress.Coins = 300;
                File.WriteAllText(temporary, JsonUtility.ToJson(interrupted, true));

                PlayerSaveData recovered = saveService.LoadOrCreate();
                Require(recovered.Progress.Coins == 300,
                    "When primary is absent, the newest complete temp save should be restored.");
                Require(File.Exists(primary), "Recovered temp save should be promoted to primary.");
                Require(!File.Exists(temporary), "Recovered temp file should be cleaned up.");
            }
            finally
            {
                if (Directory.Exists(directory)) Directory.Delete(directory, true);
            }
        }

        private static void TestContentCatalogAuthoring()
        {
            BackyardContentCatalog catalog = ScriptableObject.CreateInstance<BackyardContentCatalog>();
            try
            {
                var entry = new BackyardContentEntry
                {
                    Id = "smoke_crop",
                    Kind = BackyardContentKind.Crop,
                    DisplayName = "Smoke Crop",
                    AreaId = "backyard"
                };
                Require(catalog.TryAdd(entry), "A new stable catalog entry should be accepted.");
                Require(!catalog.TryAdd(new BackyardContentEntry
                {
                    Id = "smoke_crop",
                    Kind = BackyardContentKind.Crop,
                    DisplayName = "Duplicate",
                    AreaId = "backyard"
                }), "A duplicate catalog ID must be rejected.");
                Require(catalog.Entries.Count == 1, "Duplicate insertion must not change catalog size.");
                Require(catalog.Validate().Count == 0, "A complete basic catalog entry should validate.");
                Require(catalog.TryGet("SMOKE_CROP", out BackyardContentEntry caseInsensitive) &&
                    caseInsensitive == entry, "Catalog lookup should be case-insensitive for stable IDs.");
                Require(!catalog.TryAdd(new BackyardContentEntry
                {
                    Id = "SMOKE_CROP",
                    Kind = BackyardContentKind.Crop,
                    DisplayName = "Case Variant Duplicate",
                    AreaId = "backyard"
                }), "Case-only duplicate IDs must be rejected.");
                Require(catalog.TryGet("smoke_crop", BackyardContentKind.Crop, out _),
                    "Kind-filtered lookup should return a matching content entry.");
                Require(!catalog.TryGet("smoke_crop", BackyardContentKind.Recipe, out _),
                    "Kind-filtered lookup must reject a mismatched content kind.");
                Require(catalog.GetByKind(BackyardContentKind.Crop).Count == 1,
                    "Kind query should return exactly the crop entry.");

                entry.RequiredQuantity = 1;
                Require(catalog.Validate().Count > 0,
                    "Catalog validation should report a required quantity without a required item ID.");
                entry.RequiredQuantity = 0;
                entry.RewardQuantity = 1;
                Require(catalog.Validate().Count > 0,
                    "Catalog validation should report a reward quantity without a reward item ID.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(catalog);
            }
        }

        private static void TestTutorialFlow()
        {
            GameSession session = NewSession();
            TutorialService tutorial = new TutorialService(session);
            Require(tutorial.CurrentStep == TutorialService.Move, "New profile should begin with movement.");

            tutorial.MarkMovementSeen();
            Require(tutorial.CurrentStep == TutorialService.Plant, "Movement should advance to planting.");

            Require(session.AddItem("seed_carrot", 1), "Could not seed tutorial planting.");
            FarmingService farming = new FarmingService(session);
            Require(farming.TryPlant("tutorial_plot", FarmingService.Carrot),
                "Successful planting should complete the plant tutorial step.");
            Require(tutorial.CurrentStep == TutorialService.Harvest,
                "Planting should advance the tutorial to harvesting.");

            FarmPlotData plot = farming.GetPlot("tutorial_plot");
            plot.PlantedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 60;
            string harvestedCrop;
            Require(farming.TryHarvest("tutorial_plot", out harvestedCrop),
                "Mature tutorial crop should be harvestable.");
            Require(tutorial.CurrentStep == TutorialService.Craft,
                "Harvesting should advance the tutorial to crafting.");

            Require(session.AddItem("crop_carrot", 3), "Could not seed tutorial recipe.");
            ProductionService production = new ProductionService(session);
            Require(production.TryStart(ProductionService.CarrotSoup), "Tutorial soup should start.");
            production.ActiveJob.StartedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 10;
            production.ActiveJob.DurationSeconds = 1;
            Require(production.TryCollect(), "Tutorial soup should be collectable.");
            Require(tutorial.CurrentStep == TutorialService.Deliver,
                "Collecting a crafted product should advance to delivery.");

            OrderBoardService orders = new OrderBoardService(session);
            Require(orders.TryComplete("order_soup_01"), "Tutorial soup order should complete.");
            Require(tutorial.CurrentStep == TutorialService.Arcade,
                "Completing an order should advance to the arcade step.");

            int coins, experience;
            MiniGameRewardService arcade = new MiniGameRewardService(session);
            Require(arcade.TryClaim("tutorial_smoke_run", MiniGameRewardService.Match3, 0, out coins, out experience),
                "Tutorial arcade reward should be claimable.");
            Require(tutorial.IsCompleted && tutorial.CurrentStep == TutorialService.StepCount,
                "Claiming a mini-game reward should complete the six-step tutorial.");
        }

        private static void TestSaveRecovery()
        {
            string directory = Path.Combine(Path.GetTempPath(),
                "MiaMaxSmoke_" + Guid.NewGuid().ToString("N"));
            try
            {
                LocalSaveService saveService = new LocalSaveService(directory);
                PlayerSaveData data = new PlayerSaveData();
                data.CreatedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                data.Progress.Coins = 123;
                data.GardenLevel = 99;
                data.Progress.Level = 1000;
                data.Progress.Experience = int.MaxValue;
                data.Settings.UiScale = 4f;
                data.Inventory.Add(new MiaMax.Domain.Inventory.ItemStackData("test_stack", 2));
                data.Inventory.Add(new MiaMax.Domain.Inventory.ItemStackData("test_stack", 3));
                long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                data.FarmPlots.Add(new FarmPlotData { PlotId = "plot_a", CropId = FarmingService.Carrot, PlantedAtUnix = now });
                data.FarmPlots.Add(new FarmPlotData { PlotId = "plot_a", CropId = FarmingService.Tomato, PlantedAtUnix = now });
                data.ProductionJob = new ProductionJobData { RecipeId = "unknown_recipe", StartedAtUnix = now, DurationSeconds = 30 };
                data.CompletedOrders.Add(new CompletedOrderData { OrderId = "order_a", CompletedAtUnix = now });
                data.CompletedOrders.Add(new CompletedOrderData { OrderId = "order_a", CompletedAtUnix = now });
                data.ClaimedQuests.Add(new QuestProgressData { QuestId = "quest_a", ClaimedAtUnix = now });
                data.ClaimedQuests.Add(new QuestProgressData { QuestId = "quest_a", ClaimedAtUnix = now });

                saveService.Save(data);
                PlayerSaveData normalized = saveService.LoadOrCreate();
                Require(normalized.GardenLevel == 2, "Save validation should clamp garden level.");
                Require(normalized.Progress.Level == 100 &&
                    normalized.Progress.Experience < GameSession.ExperienceForNextLevel(100),
                    "Save validation should normalize legacy progression to the level cap.");
                Require(Math.Abs(normalized.Settings.UiScale - 1.15f) < 0.001f,
                    "Save validation should clamp UI scale.");
                Require(normalized.Inventory.Count == 1 && normalized.Inventory[0].Quantity == 5,
                    "Duplicate inventory stacks should merge into one canonical stack.");
                Require(normalized.FarmPlots.Count == 1,
                    "Duplicate farm plot IDs should be removed.");
                Require(normalized.ProductionJob == null || !normalized.ProductionJob.IsActive,
                    "Unknown production recipes should not survive save validation.");
                Require(normalized.CompletedOrders.Count == 1 && normalized.ClaimedQuests.Count == 1,
                    "Duplicate completed orders and quest claims should be removed.");

                normalized.Progress.Coins = 456;
                saveService.Save(normalized);
                string primary = Path.Combine(directory, "mia_max_save.json");
                File.WriteAllText(primary, "{ invalid json");

                PlayerSaveData recovered = saveService.LoadOrCreate();
                Require(recovered.Progress.Coins == 123,
                    "Corrupt primary save should restore the last known-good backup.");

                File.WriteAllText(primary, "{ invalid json");
                PlayerSaveData recoveredAgain = saveService.LoadOrCreate();
                Require(recoveredAgain.Progress.Coins == 123,
                    "A successful recovery must preserve the backup for a later recovery.");
            }
            finally
            {
                if (Directory.Exists(directory)) Directory.Delete(directory, true);
            }
        }

        private static void TestQuestRewardAtomicity()
        {
            GameSession session = NewSession();
            session.Data.Progress.Level = 2;
            session.Data.Progress.Coins = int.MaxValue;
            QuestService quests = new QuestService(session);
            string title;

            Require(!quests.TryClaimNext(out title), "Quest reward must reject wallet overflow.");
            Require(quests.ClaimedCount == 0, "Rejected reward must not mark the quest claimed.");
            Require(session.Data.Progress.Coins == int.MaxValue, "Rejected reward must not change coins.");

            int notifications = 0;
            bool notificationSawClaim = false;
            session.StateChanged += () =>
            {
                notifications++;
                notificationSawClaim = quests.ClaimedCount == 1;
            };
            session.Data.Progress.Coins = 0;
            session.Data.Progress.Level = 99;
            session.Data.Progress.Experience = GameSession.ExperienceForNextLevel(99) - 1;
            Require(quests.TryClaimNext(out title), "Quest should be claimable when reward capacity is available.");
            Require(notifications == 1 && notificationSawClaim,
                "Autosave listeners must observe the claim and reward together in one notification.");
            Require(quests.ClaimedCount == 1 && session.Data.Progress.Level == 100,
                "Quest should commit once and progression should stop at level 100.");
            Require(session.Data.Progress.Experience < GameSession.ExperienceForNextLevel(100),
                "XP at the level cap must remain below the next-level threshold.");
        }

        private static void TestRuntimeQuestCatalog()
        {
            BackyardContentCatalog catalog = ScriptableObject.CreateInstance<BackyardContentCatalog>();
            try
            {
                Require(catalog.TryAdd(new BackyardContentEntry
                {
                    Id = "quest_first_steps",
                    Kind = BackyardContentKind.StoryGoal,
                    DisplayName = "A Catalog Adventure",
                    AreaId = "backyard",
                    RewardCoins = 27,
                    RewardExperience = 31,
                    UnlockAreaId = "flower_garden"
                }), "Test catalog entry should be accepted.");

                GameSession session = NewSession();
                session.Data.Progress.Level = 2;
                QuestService quests = new QuestService(session, catalog);
                var snapshot = quests.GetQuestSnapshots()[0];
                Require(snapshot.Title == "A Catalog Adventure" &&
                    snapshot.RewardCoins == 27 && snapshot.RewardExperience == 31,
                    "Quest cards should resolve title and rewards from the runtime catalog.");

                string claimedTitle;
                Require(quests.TryClaimNext(out claimedTitle), "Catalog-backed quest should be claimable.");
                Require(claimedTitle == "A Catalog Adventure" &&
                    session.Data.Progress.Coins == 27 && session.Data.Progress.Experience == 31,
                    "Quest claim should use catalog title and reward values.");
                Require(quests.IsAreaUnlocked("flower_garden"),
                    "Catalog-defined quest area unlock should persist in the session.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(catalog);
            }
        }


        private static void TestCatalogDrivenProductionAndOrders()
        {
            BackyardContentCatalog catalog = ScriptableObject.CreateInstance<BackyardContentCatalog>();
            try
            {
                Require(catalog.TryAdd(new BackyardContentEntry
                {
                    Id = "recipe_custom_meal", Kind = BackyardContentKind.Recipe,
                    DisplayName = "Custom Meal", AreaId = "backyard",
                    RequiredItemId = "test_crop", RequiredQuantity = 2,
                    RewardItemId = "test_meal", RewardQuantity = 2, RewardExperience = 7
                }), "Custom recipe should be accepted.");
                Require(catalog.TryAdd(new BackyardContentEntry
                {
                    Id = "order_custom_delivery", Kind = BackyardContentKind.Order,
                    DisplayName = "Custom Delivery", AreaId = "backyard",
                    RequiredItemId = "test_meal", RequiredQuantity = 2,
                    RewardCoins = 23, RewardExperience = 9
                }), "Custom order should be accepted.");

                GameSession session = NewSession();
                Require(session.AddItem("test_crop", 2), "Could not seed custom recipe ingredients.");
                ProductionService production = new ProductionService(session, catalog);
                Require(production.TryStart("recipe_custom_meal"),
                    "Catalog-defined recipe should start with its required ingredients.");
                production.ActiveJob.StartedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 10;
                production.ActiveJob.DurationSeconds = 1;
                Require(production.TryCollect(), "Catalog-defined recipe should collect.");
                Require(session.Data.Inventory.Exists(x => x.ItemId == "test_meal" && x.Quantity == 2),
                    "Catalog recipe output item and quantity should be granted.");
                Require(session.Data.Progress.Experience == 7,
                    "Catalog recipe XP reward should be applied.");

                OrderBoardService orders = new OrderBoardService(session, catalog);
                Require(orders.CanComplete("order_custom_delivery"),
                    "Custom catalog order should appear on the order board.");
                Require(orders.TryComplete("order_custom_delivery"),
                    "Custom catalog order should consume its required quantity and pay rewards.");
                Require(!session.Data.Inventory.Exists(x => x.ItemId == "test_meal"),
                    "Order delivery should consume the configured item quantity.");
                Require(session.Data.Progress.Coins == 23 && session.Data.Progress.Experience == 16,
                    "Order coin and XP rewards should come from the catalog.");
                Require(orders.IsCompleted("order_custom_delivery"),
                    "Custom order completion should persist in session state.");
                Require(!orders.TryComplete("order_custom_delivery"),
                    "Custom order must not be claimable twice.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(catalog);
            }
        }

        private static void TestQuestPrerequisitesAndAreas()
        {
            BackyardContentCatalog catalog = ScriptableObject.CreateInstance<BackyardContentCatalog>();
            try
            {
                Require(catalog.TryAdd(new BackyardContentEntry
                {
                    Id = "quest_first_steps", Kind = BackyardContentKind.StoryGoal,
                    DisplayName = "First Steps", AreaId = "backyard",
                    RewardCoins = 5, RewardExperience = 5, UnlockAreaId = "flower_garden"
                }), "First story goal should be added.");
                Require(catalog.TryAdd(new BackyardContentEntry
                {
                    Id = "quest_first_order", Kind = BackyardContentKind.StoryGoal,
                    DisplayName = "First Delivery", AreaId = "backyard",
                    RequiredQuestId = "quest_first_steps", RewardCoins = 5, RewardExperience = 5
                }), "Second story goal should be added.");
                Require(catalog.TryAdd(new BackyardContentEntry
                {
                    Id = "recipe_flower_garden", Kind = BackyardContentKind.Recipe,
                    DisplayName = "Flower Garden Basket", AreaId = "flower_garden",
                    RequiredItemId = "flower_crop", RequiredQuantity = 1,
                    RewardItemId = "flower_basket", RewardQuantity = 1, RewardExperience = 1
                }), "Flower Garden recipe should be added.");
                Require(catalog.TryAdd(new BackyardContentEntry
                {
                    Id = "order_flower_garden", Kind = BackyardContentKind.Order,
                    DisplayName = "Flower Garden Delivery", AreaId = "flower_garden",
                    RequiredItemId = "flower_delivery", RequiredQuantity = 1,
                    RewardCoins = 5
                }), "Flower Garden order should be added.");
                Require(catalog.Validate().Count == 0,
                    "Catalog story-goal prerequisite references should validate.");

                GameSession session = NewSession();
                session.Data.Progress.Level = 2;
                QuestService quests = new QuestService(session, catalog);
                Require(session.AddItem("flower_crop", 1) && session.AddItem("flower_delivery", 1),
                    "Could not seed locked-area content fixtures.");
                ProductionService areaProduction = new ProductionService(session, catalog);
                OrderBoardService areaOrders = new OrderBoardService(session, catalog);
                Require(!areaProduction.TryStart("recipe_flower_garden"),
                    "Production in a locked area must not start.");
                Require(!areaOrders.CanComplete("order_flower_garden"),
                    "Orders in a locked area must remain unavailable.");
                var snapshots = quests.GetQuestSnapshots();
                Require(snapshots[0].Availability == QuestAvailability.ReadyToClaim,
                    "First quest should be ready when its completion condition is met.");
                Require(snapshots[1].Availability == QuestAvailability.InProgress,
                    "Second quest must remain gated until its prerequisite is claimed.");
                string title;
                Require(quests.TryClaimNext(out title) && title == "First Steps",
                    "First quest should claim before the dependent quest.");
                Require(quests.IsAreaUnlocked("flower_garden"),
                    "Claiming the catalog quest should unlock its configured area.");
                Require(areaProduction.TryStart("recipe_flower_garden"),
                    "Production should become available after its area unlock.");
                Require(areaOrders.CanComplete("order_flower_garden"),
                    "Area order should become available after its area unlock.");
                session.Data.CompletedOrders.Add(new CompletedOrderData { OrderId = "smoke_order" });
                snapshots = quests.GetQuestSnapshots();
                Require(snapshots[1].Availability == QuestAvailability.ReadyToClaim,
                    "Dependent quest should become ready after its prerequisite is claimed.");
                Require(quests.TryClaimNext(out title) && title == "First Delivery",
                    "Second quest should become claimable after its prerequisite.");
                Require(catalog.TryAdd(new BackyardContentEntry
                {
                    Id = "broken_goal", Kind = BackyardContentKind.StoryGoal,
                    DisplayName = "Broken Goal", AreaId = "backyard", RequiredQuestId = "missing_goal"
                }), "Malformed prerequisite fixture should be insertable for validation.");
                Require(catalog.Validate().Count > 0,
                    "Catalog validation should report a missing story-goal prerequisite.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(catalog);
            }
        }

        private static void TestFirstSessionLoop()
        {
            string directory = Path.Combine(Path.GetTempPath(),
                "MiaMaxFirstLoop_" + Guid.NewGuid().ToString("N"));
            try
            {
                GameSession session = NewSession();
                session.Data.Progress.Level = 2;
                QuestService quests = new QuestService(session);
                string title;
                Require(quests.TryClaimNext(out title),
                    "Fresh profile at the first quest milestone should claim its opening story reward.");

                Require(session.AddItem("crop_carrot", 3), "Could not seed the first-session crop ingredients.");
                ProductionService production = new ProductionService(session);
                Require(production.TryStart(ProductionService.CarrotSoup),
                    "First-session player should be able to start carrot soup.");
                production.ActiveJob.StartedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 10;
                production.ActiveJob.DurationSeconds = 1;
                Require(production.TryCollect(), "First-session player should collect the finished soup.");

                OrderBoardService orders = new OrderBoardService(session);
                Require(orders.TryComplete("order_soup_01"),
                    "First-session player should deliver the crafted soup order.");
                Require(quests.TryClaimNext(out title) && title == "Helping the Neighbours",
                    "First delivery should unlock the next story quest reward.");
                Require(quests.ClaimedCount == 2 && orders.IsCompleted("order_soup_01"),
                    "First-session loop should preserve both quest claims and delivery completion.");

                LocalSaveService save = new LocalSaveService(directory);
                save.Save(session.Data);
                PlayerSaveData restored = save.LoadOrCreate();
                Require(restored.ClaimedQuests.Count == 2 &&
                    restored.CompletedOrders.Exists(x => x != null && x.OrderId == "order_soup_01"),
                    "First-session progression should survive a local save/reload.");
            }
            finally
            {
                if (Directory.Exists(directory)) Directory.Delete(directory, true);
            }
        }


        private static void TestInventoryService()
        {
            GameSession session = NewSession();
            InventoryService inventory = new InventoryService(session);
            Require(inventory.TryAdd("test_berry", 5), "Inventory service should add a valid stack.");
            Require(inventory.Count == 1 && inventory.GetQuantity("test_berry") == 5,
                "Inventory facade should expose canonical stack count and quantity.");
            Require(inventory.Has("test_berry", 5) && !inventory.Has("test_berry", 6),
                "Inventory availability checks should compare the requested quantity.");
            var snapshot = inventory.GetSnapshot();
            Require(snapshot.Count == 1 && snapshot[0].Quantity == 5,
                "Inventory snapshot should contain a detached copy of the stack.");
            snapshot[0].Quantity = 999;
            Require(inventory.GetQuantity("test_berry") == 5,
                "Mutating a read snapshot must not mutate saved inventory.");
            Require(inventory.TryConsume("test_berry", 2) && inventory.GetQuantity("test_berry") == 3,
                "Inventory service should delegate validated consumption to the session.");
            Require(!inventory.TryConsume("test_berry", 4),
                "Inventory service must reject consumption beyond the available stack.");
        }

        private static void TestSharedProgressionTransactions()
        {
            GameSession upgradeSession = NewSession();
            Require(upgradeSession.TryAddCoins(200), "Could not seed upgrade wallet.");
            UpgradeService upgrades = new UpgradeService(upgradeSession);
            int upgradeNotifications = 0;
            upgradeSession.StateChanged += () => upgradeNotifications++;
            Require(upgrades.TryUpgradeGarden(), "Garden upgrade should purchase successfully.");
            Require(upgradeNotifications == 1 && upgradeSession.Data.Progress.Coins == 140 &&
                upgradeSession.Data.GardenLevel == 1,
                "Upgrade cost and level must commit in one notification.");

            GameSession dailySession = NewSession();
            dailySession.Data.Progress.Level = 99;
            dailySession.Data.Progress.Experience = GameSession.ExperienceForNextLevel(99) - 1;
            DailyTaskService daily = new DailyTaskService(dailySession);
            daily.RecordHarvest(); daily.RecordHarvest();
            daily.RecordCraft(); daily.RecordDelivery(); daily.RecordExploration();
            int dailyNotifications = 0;
            dailySession.StateChanged += () => dailyNotifications++;
            Require(daily.TryClaimReward(), "Completed daily goals should grant their reward.");
            Require(dailyNotifications == 1 && daily.IsRewardClaimed &&
                dailySession.Data.Progress.Level == 100 &&
                dailySession.Data.Progress.Experience < GameSession.ExperienceForNextLevel(100),
                "Daily reward must use the shared level cap and commit exactly once.");

            GameSession achievementSession = NewSession();
            achievementSession.Data.Progress.Level = 100;
            achievementSession.Data.Progress.Experience = GameSession.ExperienceForNextLevel(100) - 1;
            AchievementService achievements = new AchievementService(achievementSession);
            achievementSession.Data.Achievements.Add(new AchievementProgressData
            {
                AchievementId = "harvest_10", Progress = 10
            });
            int achievementNotifications = 0;
            achievementSession.StateChanged += () => achievementNotifications++;
            string title;
            int coins, experience;
            Require(achievements.TryClaimNext(out title, out coins, out experience),
                "Completed lifetime achievement should grant its reward.");
            Require(achievementNotifications == 1 && title == "Green Fingers" &&
                achievementSession.Data.Progress.Level == 100 &&
                achievementSession.Data.Progress.Experience < GameSession.ExperienceForNextLevel(100),
                "Achievement reward must use the shared level cap and commit exactly once.");
        }

        private static GameSession NewSession()
        {
            PlayerSaveData data = new PlayerSaveData();
            data.CreatedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            data.UpdatedAtUnix = data.CreatedAtUnix;
            return new GameSession(data);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
#endif
