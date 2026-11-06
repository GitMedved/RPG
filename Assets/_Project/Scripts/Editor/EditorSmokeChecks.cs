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
                RunCheck("Six-step tutorial gameplay flow", TestTutorialFlow, ref passed);

                string message = "Core smoke checks passed: " + passed + "/7.\n\n"+
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
        }

        private static void TestProductionAndOrders()
        {
            GameSession session = NewSession();
            DailyTaskService daily = new DailyTaskService(session);
            Require(session.AddItem("crop_carrot", 3), "Could not seed production ingredients.");

            ProductionService production = new ProductionService(session);
            Require(production.TryStart(ProductionService.CarrotSoup), "Soup production should start.");
            Require(!production.TryStart(ProductionService.CarrotSoup),
                "A second production job must not start while the slot is occupied.");

            production.ActiveJob.StartedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 10;
            production.ActiveJob.DurationSeconds = 1;
            Require(production.TryCollect(), "Finished soup should be collectable.");
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
