using System;
using MiaMax.Domain;
using MiaMax.Infrastructure.Save;

namespace MiaMax.Domain.Progression
{
    /// <summary>Three deterministic daily goals, reset by UTC calendar date.</summary>
    public sealed class DailyTaskService
    {
        public const int HarvestGoal = 2;
        public const int CraftGoal = 1;
        public const int DeliverGoal = 1;
        public const int RewardCoins = 20;
        public const int RewardExperience = 15;

        private readonly GameSession session;
        private DailyTaskSaveData Data => session.Data.DailyTasks;

        public DailyTaskService(GameSession session)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            if (session.Data.DailyTasks == null) session.Data.DailyTasks = new DailyTaskSaveData();
            ResetForToday();
        }

        public bool IsComplete => Data.HarvestCount >= HarvestGoal &&
                                  Data.CraftCount >= CraftGoal &&
                                  Data.DeliverCount >= DeliverGoal;
        public bool IsRewardClaimed => Data.RewardClaimed;

        public void RecordHarvest()
        {
            ResetForToday();
            Data.HarvestCount = Math.Min(HarvestGoal, Data.HarvestCount + 1);
            session.NotifyExternalStateChanged();
        }

        public void RecordCraft()
        {
            ResetForToday();
            Data.CraftCount = Math.Min(CraftGoal, Data.CraftCount + 1);
            session.NotifyExternalStateChanged();
        }

        public void RecordDelivery()
        {
            ResetForToday();
            Data.DeliverCount = Math.Min(DeliverGoal, Data.DeliverCount + 1);
            session.NotifyExternalStateChanged();
        }

        public bool TryClaimReward()
        {
            ResetForToday();
            if (!IsComplete || Data.RewardClaimed) return false;
            if (session.Data.Progress.Coins > int.MaxValue - RewardCoins ||
                session.Data.Progress.Experience > int.MaxValue - RewardExperience) return false;

            Data.RewardClaimed = true;
            session.TryAddCoins(RewardCoins);
            session.AddExperience(RewardExperience);
            session.NotifyExternalStateChanged();
            return true;
        }

        public string GetSummary()
        {
            ResetForToday();
            string status = Data.RewardClaimed ? "Reward claimed" :
                IsComplete ? "All goals complete — claim your reward!" : "Complete today's goals";
            return "DAILY GOALS (UTC)\nHarvest " + Data.HarvestCount + "/" + HarvestGoal +
                   "  •  Craft " + Data.CraftCount + "/" + CraftGoal +
                   "  •  Deliver " + Data.DeliverCount + "/" + DeliverGoal +
                   "\n" + status + "  |  Reward: +" + RewardCoins + " coins, +" + RewardExperience + " XP";
        }

        private void ResetForToday()
        {
            string today = DateTime.UtcNow.ToString("yyyy-MM-dd");
            // ISO dates sort lexicographically; ignore clock rollback to avoid resetting progress repeatedly.
            if (!string.IsNullOrEmpty(Data.UtcDateKey) && string.CompareOrdinal(today, Data.UtcDateKey) <= 0) return;
            Data.UtcDateKey = today;
            Data.HarvestCount = 0;
            Data.CraftCount = 0;
            Data.DeliverCount = 0;
            Data.RewardClaimed = false;
            session.NotifyExternalStateChanged();
        }
    }
}
