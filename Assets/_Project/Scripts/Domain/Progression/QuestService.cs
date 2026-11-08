using System;
using System.Collections.Generic;
using System.Linq;
using MiaMax.Infrastructure.Save;
using UnityEngine;

namespace MiaMax.Domain.Progression
{
    public enum QuestAvailability
    {
        InProgress,
        ReadyToClaim,
        Claimed
    }

    public sealed class QuestSnapshot
    {
        public string Id { get; internal set; }
        public string Title { get; internal set; }
        public QuestAvailability Availability { get; internal set; }
        public int RewardCoins { get; internal set; }
        public int RewardExperience { get; internal set; }
    }

    /// <summary>
    /// Small, deterministic starter quest chain based on existing level and order systems.
    /// Rewards can only be claimed once and all state is stored in the local player save.
    /// </summary>
    public sealed class QuestService
    {
        private sealed class Definition
        {
            public string Id;
            public string Title;
            public int Coins;
            public int Experience;
            public Func<PlayerSaveData, bool> IsComplete;
        }

        private static readonly Definition[] Quests =
        {
            new Definition { Id = "quest_first_steps", Title = "A New Adventure", Coins = 10, Experience = 15,
                IsComplete = save => save.Progress.Level >= 2 },
            new Definition { Id = "quest_first_order", Title = "Helping the Neighbours", Coins = 15, Experience = 20,
                IsComplete = save => save.CompletedOrders.Any(x => x != null) },
            new Definition { Id = "quest_order_master", Title = "A Busy Little Backyard", Coins = 30, Experience = 35,
                IsComplete = save => save.CompletedOrders.Count(x => x != null) >= 3 },
            new Definition { Id = "quest_flower_friend", Title = "A Path Beyond the Garden", Coins = 40, Experience = 45,
                IsComplete = save => save.CollectedGardenResources != null && save.CollectedGardenResources.Count >= 3 }
        };

        private readonly GameSession session;
        private readonly BackyardContentCatalog catalog;

        public QuestService(GameSession session) : this(session,
            Resources.Load<BackyardContentCatalog>("MiaMax/Content/BackyardContentCatalog"))
        {
        }

        public QuestService(GameSession session, BackyardContentCatalog catalog)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            this.catalog = catalog;
            if (session.Data.ClaimedQuests == null) session.Data.ClaimedQuests = new List<QuestProgressData>();
            if (session.Data.UnlockedWorldAreas == null) session.Data.UnlockedWorldAreas = new List<string>();
        }

        public int CompletedCount => Quests.Count(x => x.IsComplete(session.Data));
        public int ClaimedCount => session.Data.ClaimedQuests.Count(x => x != null);

        public IReadOnlyList<QuestSnapshot> GetQuestSnapshots()
        {
            var snapshots = new List<QuestSnapshot>(Quests.Length);
            foreach (Definition quest in Quests)
            {
                bool claimed = IsClaimed(quest.Id);
                bool complete = quest.IsComplete(session.Data);
                snapshots.Add(new QuestSnapshot
                {
                    Id = quest.Id,
                    Title = GetTitle(quest),
                    Availability = claimed ? QuestAvailability.Claimed :
                        complete ? QuestAvailability.ReadyToClaim : QuestAvailability.InProgress,
                    RewardCoins = GetCoins(quest),
                    RewardExperience = GetExperience(quest)
                });
            }
            return snapshots;
        }

        public string GetSummary()
        {
            int claimable = Quests.Count(x => x.IsComplete(session.Data) && !IsClaimed(x.Id));
            return "Story quests: " + ClaimedCount + "/" + Quests.Length +
                " rewards claimed. Ready to claim: " + claimable +
                ". Interact to claim the next reward.";
        }

        public bool TryClaimNext(out string claimedTitle)
        {
            claimedTitle = null;
            Definition quest = Quests.FirstOrDefault(x => x.IsComplete(session.Data) && !IsClaimed(x.Id));
            if (quest == null) return false;

            // Prepare all persistent quest state before the single reward notification.
            // The preflight makes the following grant deterministic on Unity's main thread.
            int rewardCoins = GetCoins(quest);
            int rewardExperience = GetExperience(quest);
            if (!session.CanGrantRewards(rewardCoins, rewardExperience)) return false;
            session.Data.ClaimedQuests.Add(new QuestProgressData
            {
                QuestId = quest.Id,
                ClaimedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            });
            string unlockArea = GetUnlockArea(quest);
            if (!string.IsNullOrWhiteSpace(unlockArea) && !session.Data.UnlockedWorldAreas.Contains(unlockArea))
                session.Data.UnlockedWorldAreas.Add(unlockArea);

            if (!session.TryGrantRewards(rewardCoins, rewardExperience))
            {
                session.Data.ClaimedQuests.RemoveAt(session.Data.ClaimedQuests.Count - 1);
                return false;
            }
            claimedTitle = GetTitle(quest);
            return true;
        }

        public bool IsAreaUnlocked(string areaId)
        {
            return !string.IsNullOrWhiteSpace(areaId) &&
                session.Data.UnlockedWorldAreas.Contains(areaId);
        }

        private string GetTitle(Definition quest)
        {
            return TryGetCatalogEntry(quest, out BackyardContentEntry entry) &&
                !string.IsNullOrWhiteSpace(entry.DisplayName) ? entry.DisplayName : quest.Title;
        }

        private int GetCoins(Definition quest)
        {
            return TryGetCatalogEntry(quest, out BackyardContentEntry entry) ? entry.RewardCoins : quest.Coins;
        }

        private int GetExperience(Definition quest)
        {
            return TryGetCatalogEntry(quest, out BackyardContentEntry entry) ? entry.RewardExperience : quest.Experience;
        }

        private string GetUnlockArea(Definition quest)
        {
            if (TryGetCatalogEntry(quest, out BackyardContentEntry entry) &&
                !string.IsNullOrWhiteSpace(entry.UnlockAreaId))
                return entry.UnlockAreaId;
            if (quest.Id == "quest_order_master") return "flower_garden";
            if (quest.Id == "quest_flower_friend") return "woodland_trail";
            return null;
        }

        private bool TryGetCatalogEntry(Definition quest, out BackyardContentEntry entry)
        {
            return catalog != null && catalog.TryGet(quest.Id, BackyardContentKind.StoryGoal, out entry);
        }

        private bool IsClaimed(string id)
        {
            return session.Data.ClaimedQuests.Any(x => x != null && x.QuestId == id);
        }
    }
}
