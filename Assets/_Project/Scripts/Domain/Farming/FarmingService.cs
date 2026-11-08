using System;
using System.Collections.Generic;
using System.Linq;
using MiaMax.Domain.Progression;
using MiaMax.Infrastructure.Save;
namespace MiaMax.Domain.Farming
{
    public sealed class FarmingService
    {
        public const string Carrot = "crop_carrot";
        public const string Tomato = "crop_tomato";
        public const string Strawberry = "crop_strawberry";
        private readonly GameSession session;
        private readonly PlayerSaveData save;
        public FarmingService(GameSession session)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            save = session.Data;
            if (save.FarmPlots == null) save.FarmPlots = new List<FarmPlotData>();
        }
        public FarmPlotData GetPlot(string id)
        {
            var plot = save.FarmPlots.FirstOrDefault(p => p != null && p.PlotId == id);
            if (plot == null) { plot = new FarmPlotData { PlotId = id }; save.FarmPlots.Add(plot); }
            return plot;
        }
        public bool TryPlant(string plotId, string cropId)
        {
            if (!IsValidCrop(cropId)) return false;
            var plot = GetPlot(plotId);
            if (plot.IsPlanted || !session.TryRemoveItem(SeedFor(cropId), 1)) return false;
            plot.CropId = cropId;
            plot.PlantedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            session.AddExperience(EconomyBalance.PlantExperience);
            new TutorialService(session).TryAdvance(TutorialService.Plant);
            session.NotifyExternalStateChanged();
            return true;
        }
        public bool TryHarvest(string plotId, out string cropId)
        {
            var plot = GetPlot(plotId);
            cropId = plot.CropId;
            if (!plot.IsPlanted || GrowthStage(plot) < 3) return false;
            int harvestQuantity = new UpgradeService(session).HarvestYield;
            if (!session.CanGrantItemAndRewards(cropId, harvestQuantity,
                EconomyBalance.HarvestCoins, EconomyBalance.HarvestExperience)) return false;

            // Clear the plot before the single inventory/reward notification. Roll back if preflight changes.
            string harvestedCrop = plot.CropId;
            long plantedAt = plot.PlantedAtUnix;
            plot.CropId = null;
            plot.PlantedAtUnix = 0;
            if (!session.TryGrantItemAndRewards(harvestedCrop, harvestQuantity,
                EconomyBalance.HarvestCoins, EconomyBalance.HarvestExperience))
            {
                plot.CropId = harvestedCrop;
                plot.PlantedAtUnix = plantedAt;
                return false;
            }
            new DailyTaskService(session).RecordHarvest();
            new AchievementService(session).RecordHarvest();
            new TutorialService(session).TryAdvance(TutorialService.Harvest);
            session.NotifyExternalStateChanged();
            return true;
        }
        public int GrowthStage(FarmPlotData plot)
        {
            if (plot == null || !plot.IsPlanted) return 0;
            long elapsed = Math.Max(0, DateTimeOffset.UtcNow.ToUnixTimeSeconds() - plot.PlantedAtUnix);
            int minutes = GrowMinutes(plot.CropId);
            long totalSeconds = minutes * 60L;
            if (elapsed >= totalSeconds) return 3;
            return 1 + (int)(elapsed / (totalSeconds / 2f));
        }
        public static bool IsValidCrop(string id) => id == Carrot || id == Tomato || id == Strawberry;
        public static string SeedFor(string crop) => "seed_" + crop.Substring(5);
        private static int GrowMinutes(string crop) => crop == Strawberry ? 3 : crop == Tomato ? 2 : 1;
    }
}