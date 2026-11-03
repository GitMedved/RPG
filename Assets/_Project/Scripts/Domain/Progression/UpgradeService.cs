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
            if (GardenAtMax || !session.TrySpendCoins(GardenUpgradeCost)) return false;
            session.Data.GardenLevel++;
            session.NotifyExternalStateChanged();
            return true;
        }

        public bool TryUpgradeWorkshop()
        {
            if (WorkshopAtMax || !session.TrySpendCoins(WorkshopUpgradeCost)) return false;
            session.Data.WorkshopLevel++;
            session.NotifyExternalStateChanged();
            return true;
        }

        /// <summary>Base harvest is two crops; each garden level adds one.</summary>
        public int HarvestYield => 2 + GardenLevel;

        /// <summary>Workshop level 1 is 15% faster; level 2 is 30% faster.</summary>
        public int AdjustProductionDuration(int baseSeconds)
        {
            if (baseSeconds <= 0) return 0;
            float multiplier = WorkshopLevel == 1 ? 0.85f : WorkshopLevel >= 2 ? 0.70f : 1f;
            return Math.Max(1, (int)Math.Ceiling(baseSeconds * multiplier));
        }

        private static int CostFor(int currentLevel)
        {
            return currentLevel <= 0 ? 60 : currentLevel == 1 ? 120 : int.MaxValue;
        }
    }
}
