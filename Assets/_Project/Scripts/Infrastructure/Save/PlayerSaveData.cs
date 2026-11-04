using System;
using System.Collections.Generic;
using MiaMax.Domain.Inventory;
using MiaMax.Domain.Progression;
using MiaMax.Domain.Farming;
using MiaMax.Domain.Production;
using MiaMax.Domain.Orders;
using MiaMax.Domain.MiniGames;

namespace MiaMax.Infrastructure.Save
{
    [Serializable]
    public sealed class DailyTaskSaveData
    {
        public string UtcDateKey;
        public int HarvestCount;
        public int CraftCount;
        public int DeliverCount;
        public int ExploreCount;
        public bool RewardClaimed;
    }

    [Serializable]
    public sealed class DailyVisitSaveData
    {
        public string LastVisitUtcDate;
        public string RewardClaimedUtcDate;
        public int CurrentStreak;
        public int BestStreak;
    }

    [Serializable]
    public sealed class PlayerSaveData
    {
        public const int CurrentSchemaVersion = 1;

        public int SchemaVersion = CurrentSchemaVersion;
        public long CreatedAtUnix;
        public long UpdatedAtUnix;
        public bool StarterKitGranted;
        public PlayerProgressData Progress = new PlayerProgressData();
        public List<ItemStackData> Inventory = new List<ItemStackData>();
        public List<FarmPlotData> FarmPlots = new List<FarmPlotData>();
        public ProductionJobData ProductionJob = new ProductionJobData();
        public List<CompletedOrderData> CompletedOrders = new List<CompletedOrderData>();
        public List<QuestProgressData> ClaimedQuests = new List<QuestProgressData>();
        public List<string> UnlockedWorldAreas = new List<string>();
        public List<string> CollectedGardenResources = new List<string>();
        public List<string> CollectedTrailResources = new List<string>();
        public List<MiniGameRewardClaimData> MiniGameRewardClaims = new List<MiniGameRewardClaimData>();
        public List<string> DiscoveredCollectibles = new List<string>();
        public bool CollectionCompletionRewardClaimed;
        public int TutorialStep;
        public bool TutorialMovementSeen;
        public bool TutorialCompleted;
        public int GardenLevel;
        public int WorkshopLevel;
        public PlayerSettingsData Settings = new PlayerSettingsData();
        public DailyTaskSaveData DailyTasks = new DailyTaskSaveData();
        public DailyVisitSaveData DailyVisit = new DailyVisitSaveData();
        public List<AchievementProgressData> Achievements = new List<AchievementProgressData>();
    }
}
