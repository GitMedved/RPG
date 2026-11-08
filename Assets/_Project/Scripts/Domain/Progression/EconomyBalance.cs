namespace MiaMax.Domain.Progression
{
    /// <summary>Single source of truth for first-release economy and upgrade tuning.</summary>
    public static class EconomyBalance
    {
        public const int MaxUpgradeLevel = 2;
        public const int BaseHarvestYield = 2;
        public const int HarvestYieldPerGardenLevel = 1;
        public const int HarvestCoins = 3;
        public const int HarvestExperience = 8;
        public const int PlantExperience = 2;

        public static int UpgradeCost(int currentLevel)
        {
            if (currentLevel < 0) return int.MaxValue;
            if (currentLevel == 0) return 60;
            if (currentLevel == 1) return 120;
            return int.MaxValue;
        }

        public static int HarvestYield(int gardenLevel)
        {
            int level = ClampLevel(gardenLevel);
            return BaseHarvestYield + level * HarvestYieldPerGardenLevel;
        }

        public static int ProductionDurationPercent(int workshopLevel)
        {
            int level = ClampLevel(workshopLevel);
            return level == 1 ? 85 : level >= 2 ? 70 : 100;
        }

        public static int AdjustProductionDuration(int baseSeconds, int workshopLevel)
        {
            if (baseSeconds <= 0) return 0;
            long scaled = (long)baseSeconds * ProductionDurationPercent(workshopLevel);
            long rounded = (scaled + 99L) / 100L;
            return (int)(rounded > int.MaxValue ? int.MaxValue : rounded < 1L ? 1L : rounded);
        }

        private static int ClampLevel(int level)
        {
            return level < 0 ? 0 : level > MaxUpgradeLevel ? MaxUpgradeLevel : level;
        }
    }
}