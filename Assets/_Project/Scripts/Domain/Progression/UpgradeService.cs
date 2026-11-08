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
        public const int MaxLevel = EconomyBalance.MaxUpgradeLevel;
        private readonly GameSession session;

        public UpgradeService(GameSession session)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public int GardenLevel => session.Data.GardenLevel;
        public int WorkshopLevel => session.Data.WorkshopLevel;
        public int GardenUpgradeCost => EconomyBalance.UpgradeCost(GardenLevel);
        public int WorkshopUpgradeCost => EconomyBalance.UpgradeCost(WorkshopLevel);
        public bool GardenAtMax => GardenLevel >= MaxLevel;
        public bool CanUpgradeGarden => !GardenAtMax && session.Data.Progress.Coins >= GardenUpgradeCost;
        public bool CanUpgradeWorkshop => !WorkshopAtMax && session.Data.Progress.Coins >= WorkshopUpgradeCost;
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
        public int HarvestYield => EconomyBalance.HarvestYield(GardenLevel);
        public int NextGardenHarvestYield => EconomyBalance.HarvestYield(GardenLevel + 1);
        public int NextWorkshopDuration(int baseSeconds) => EconomyBalance.AdjustProductionDuration(baseSeconds, WorkshopLevel + 1);

        /// <summary>Workshop level 1 is 15% faster; level 2 is 30% faster.</summary>
        public int AdjustProductionDuration(int baseSeconds)
        {
            return EconomyBalance.AdjustProductionDuration(baseSeconds, WorkshopLevel);
        }

    }
}
