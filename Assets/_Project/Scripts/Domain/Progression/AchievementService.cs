using System;
using System.Collections.Generic;
using System.Linq;
using MiaMax.Infrastructure.Save;

namespace MiaMax.Domain.Progression
{
    /// <summary>Lifetime achievements with one-time, locally persisted rewards.</summary>
    public sealed class AchievementService
    {
        private sealed class Definition
        {
            public string Id;
            public string Title;
            public string Description;
            public int Goal;
            public int Coins;
            public int Experience;
        }

        private static readonly Definition[] Definitions =
        {
            new Definition { Id = "harvest_10", Title = "Green Fingers", Description = "Harvest 10 crops", Goal = 10, Coins = 20, Experience = 15 },
            new Definition { Id = "craft_5", Title = "Little Maker", Description = "Collect 5 crafted products", Goal = 5, Coins = 25, Experience = 20 },
            new Definition { Id = "deliver_5", Title = "Helpful Neighbour", Description = "Deliver 5 orders", Goal = 5, Coins = 30, Experience = 25 },
            new Definition { Id = "keepsakes_5", Title = "Memory Keeper", Description = "Find all 5 backyard keepsakes", Goal = 5, Coins = 35, Experience = 25 },
            new Definition { Id = "flowers_3", Title = "Flower Friend", Description = "Collect all 3 garden flowers", Goal = 3, Coins = 20, Experience = 20 },
            new Definition { Id = "arcade_3", Title = "Arcade Explorer", Description = "Claim 3 mini-game rewards", Goal = 3, Coins = 25, Experience = 20 },
            new Definition { Id = "level_5", Title = "Rising Star", Description = "Reach player level 5", Goal = 5, Coins = 40, Experience = 30 }
        };

        private readonly GameSession session;
        public AchievementService(GameSession session)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            if (session.Data.Achievements == null) session.Data.Achievements = new List<AchievementProgressData>();
            RefreshDerivedProgress();
        }

        public int CompletedCount => Definitions.Count(x => GetProgress(x.Id) >= x.Goal);
        public int ClaimedCount => Definitions.Count(x => IsClaimed(x.Id));
        public int TotalCount => Definitions.Length;

        public void RecordHarvest() { Record("harvest_10", 1); }
        public void RecordCraft() { Record("craft_5", 1); }
        public void RecordDelivery() { Record("deliver_5", 1); }
        public void RecordArcadeReward() { Record("arcade_3", 1); }

        public bool TryClaimNext(out string title, out int coins, out int experience)
        {
            title = null; coins = 0; experience = 0;
            RefreshDerivedProgress();
            Definition definition = Definitions.FirstOrDefault(x => GetProgress(x.Id) >= x.Goal && !IsClaimed(x.Id));
            if (definition == null) return false;
            if (session.Data.Progress.Coins > int.MaxValue - definition.Coins ||
                session.Data.Progress.Experience > int.MaxValue - definition.Experience) return false;

            AchievementProgressData state = GetOrCreate(definition.Id);
            state.RewardClaimed = true;
            session.Data.Progress.Coins += definition.Coins;
            session.Data.Progress.Experience += definition.Experience;
            while (session.Data.Progress.Experience >= GameSession.ExperienceForNextLevel(session.Data.Progress.Level))
            {
                session.Data.Progress.Experience -= GameSession.ExperienceForNextLevel(session.Data.Progress.Level);
                session.Data.Progress.Level++;
            }
            session.NotifyExternalStateChanged();
            title = definition.Title; coins = definition.Coins; experience = definition.Experience;
            return true;
        }

        public string GetSummary()
        {
            RefreshDerivedProgress();
            int ready = Definitions.Count(x => GetProgress(x.Id) >= x.Goal && !IsClaimed(x.Id));
            return "Achievements: " + CompletedCount + "/" + TotalCount + " completed, " +
                ClaimedCount + " rewards claimed. " + (ready > 0 ? ready + " reward(s) ready to claim." : "Keep exploring!");
        }

        public string GetDetails(int maximum)
        {
            RefreshDerivedProgress();
            var lines = new List<string>();
            foreach (Definition definition in Definitions)
            {
                if (lines.Count >= Math.Max(1, maximum)) break;
                int progress = Math.Min(definition.Goal, GetProgress(definition.Id));
                string status = IsClaimed(definition.Id) ? " — claimed" :
                    progress >= definition.Goal ? " — reward ready" : "";
                lines.Add(definition.Title + ": " + progress + "/" + definition.Goal + status);
            }
            return string.Join("\n", lines.ToArray());
        }

        private void Record(string id, int amount)
        {
            if (amount <= 0) return;
            Definition definition = Definitions.FirstOrDefault(x => x.Id == id);
            if (definition == null) return;
            AchievementProgressData state = GetOrCreate(id);
            int next = Math.Min(definition.Goal, state.Progress + amount);
            if (next == state.Progress) return;
            state.Progress = next;
            session.NotifyExternalStateChanged();
        }

        private void RefreshDerivedProgress()
        {
            bool changed = false;
            changed |= RecordDerived("keepsakes_5", session.Data.DiscoveredCollectibles != null ? session.Data.DiscoveredCollectibles.Count : 0);
            changed |= RecordDerived("flowers_3", session.Data.CollectedGardenResources != null ? session.Data.CollectedGardenResources.Count : 0);
            changed |= RecordDerived("arcade_3", session.Data.MiniGameRewardClaims != null ? session.Data.MiniGameRewardClaims.Count : 0);
            changed |= RecordDerived("level_5", session.Data.Progress != null ? session.Data.Progress.Level : 1);
            if (changed) session.NotifyExternalStateChanged();
        }

        private bool RecordDerived(string id, int value)
        {
            Definition definition = Definitions.FirstOrDefault(x => x.Id == id);
            if (definition == null) return false;
            AchievementProgressData state = GetOrCreate(id);
            int next = Math.Min(definition.Goal, Math.Max(0, value));
            if (state.Progress >= next) return false;
            state.Progress = next;
            return true;
        }

        private int GetProgress(string id)
        {
            AchievementProgressData state = session.Data.Achievements.FirstOrDefault(x => x != null && x.AchievementId == id);
            return state != null ? state.Progress : 0;
        }

        private bool IsClaimed(string id)
        {
            AchievementProgressData state = session.Data.Achievements.FirstOrDefault(x => x != null && x.AchievementId == id);
            return state != null && state.RewardClaimed;
        }

        private AchievementProgressData GetOrCreate(string id)
        {
            AchievementProgressData state = session.Data.Achievements.FirstOrDefault(x => x != null && x.AchievementId == id);
            if (state == null)
            {
                state = new AchievementProgressData { AchievementId = id };
                session.Data.Achievements.Add(state);
            }
            return state;
        }
    }
}
