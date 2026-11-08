using System;
using MiaMax.Domain;

namespace MiaMax.Domain.Progression
{
    /// <summary>
    /// Persistent, deterministic upgrades for the garden and workshop.
    /// Upgrade costs are paid once and their effects apply to future actions.
    /// </summary>
    public sealed class UpgradeService
    {
        public const int MaxLevel = 2;
        private readonly GameSession session;

        public UpgradeService(GameSession session)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public int GardenLevel => session.Data.GardenLevel;
        public int WorkshopLevel => session.Data.WorkshopLevel;
        public int GardenUpgradeCost => CostFor(GardenLevel);
        public int WorkshopUpgradeCost => CostFor(WorkshopLevel);
        public bool GardenAtMax => GardenLevel >= MaxLevel;
        public bool WorkshopAtMax => WorkshopLevel >= MaxLevel;

        public bool TryUpgradeGarden()
        {
            return session.TryPurchaseUpgrade(GardenUpgradeCost, true, MaxLevel);
        }

        public bool TryUpgradeWorkshop()
        {
            return session.TryPurchaseUpgrade(WorkshopUpgradeCost, false, MaxLevel);
        }

        /// <summary>Base harvest is two crops; each garden level adds one.</summary>
        public int HarvestYield => 2 + GardenLevel;

        /// <summary>Workshop level 1 is 15% faster; level 2 is 30% faster.</summary>
        public int AdjustProductionDuration(int baseSeconds)
        {
            if (baseSeconds <= 0) return 0;
            // Integer ceiling avoids float representation turning 180 * 0.85 into 154.
            int percent = WorkshopLevel == 1 ? 85 : WorkshopLevel >= 2 ? 70 : 100;
            long scaledSeconds = (long)baseSeconds * percent;
            long roundedSeconds = (scaledSeconds + 99L) / 100L;
            return (int)Math.Max(1L, Math.Min(int.MaxValue, roundedSeconds));
        }

        private static int CostFor(int currentLevel)
        {
            return currentLevel <= 0 ? 60 : currentLevel == 1 ? 120 : int.MaxValue;
        }
    }
}
