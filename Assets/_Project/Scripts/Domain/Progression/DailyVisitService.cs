using System;
using System.Globalization;
using MiaMax.Domain;
using MiaMax.Infrastructure.Save;

namespace MiaMax.Domain.Progression
{
    /// <summary>
    /// Offline daily visit streak and seven-day repeating reward track.
    /// Uses UTC dates and persists a claim date to prevent duplicate same-day claims.
    /// </summary>
    public sealed class DailyVisitService
    {
        private static readonly int[] CoinRewards = { 10, 15, 20, 25, 30, 40, 60 };
        private static readonly int[] ExperienceRewards = { 5, 5, 10, 10, 15, 20, 30 };

        private readonly GameSession session;
        private DailyVisitSaveData Data => session.Data.DailyVisit;

        public DailyVisitService(GameSession session)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            if (session.Data.DailyVisit == null)
                session.Data.DailyVisit = new DailyVisitSaveData();
            RefreshForToday();
        }

        public int CurrentStreak { get { RefreshForToday(); return Data.CurrentStreak; } }
        public int BestStreak { get { RefreshForToday(); return Data.BestStreak; } }
        public int RewardDay { get { RefreshForToday(); return GetRewardDay(); } }
        public int CoinsToday { get { RefreshForToday(); return CoinRewards[GetRewardDay() - 1]; } }
        public int ExperienceToday { get { RefreshForToday(); return ExperienceRewards[GetRewardDay() - 1]; } }
        public bool IsRewardClaimed
        {
            get
            {
                RefreshForToday();
                return Data.RewardClaimedUtcDate == TodayKey();
            }
        }

        public bool CanClaimToday
        {
            get
            {
                RefreshForToday();
                return Data.LastVisitUtcDate == TodayKey() &&
                       Data.RewardClaimedUtcDate != TodayKey();
            }
        }

        public bool TryClaimReward()
        {
            RefreshForToday();
            if (!CanClaimToday) return false;

            int coins = CoinRewards[GetRewardDay() - 1];
            int experience = ExperienceRewards[GetRewardDay() - 1];
            if (session.Data.Progress.Coins > int.MaxValue - coins ||
                session.Data.Progress.Experience > int.MaxValue - experience) return false;

            Data.RewardClaimedUtcDate = TodayKey();
            session.Data.Progress.Coins += coins;
            session.Data.Progress.Experience += experience;
            while (session.Data.Progress.Experience >= GameSession.ExperienceForNextLevel(session.Data.Progress.Level))
            {
                session.Data.Progress.Experience -= GameSession.ExperienceForNextLevel(session.Data.Progress.Level);
                session.Data.Progress.Level++;
            }
            session.NotifyExternalStateChanged();
            return true;
        }

        public string GetButtonLabel()
        {
            RefreshForToday();
            string state = IsRewardClaimed ? "CLAIMED" : CanClaimToday ? "CLAIM GIFT" : "UTC DATE CHECK";
            return "DAY " + GetRewardDay() + "  ·  " + state;
        }

        public string GetSummary()
        {
            RefreshForToday();
            return "VISIT STREAK: " + Data.CurrentStreak + " day(s)  ·  BEST: " + Data.BestStreak +
                   "\nDay " + GetRewardDay() + " gift: +" + CoinsToday + " coins, +" + ExperienceToday + " XP" +
                   (IsRewardClaimed ? "\nToday's gift claimed." :
                    CanClaimToday ? "\nYour daily gift is ready!" : "\nReturn on the next UTC day to continue.");
        }

        private void RefreshForToday()
        {
            string today = TodayKey();
            if (!string.IsNullOrEmpty(Data.LastVisitUtcDate) &&
                string.CompareOrdinal(today, Data.LastVisitUtcDate) <= 0) return;

            string yesterday = DateTime.UtcNow.Date.AddDays(-1).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            Data.CurrentStreak = Data.LastVisitUtcDate == yesterday
                ? Math.Min(36500, Data.CurrentStreak + 1)
                : 1;
            Data.LastVisitUtcDate = today;
            if (Data.CurrentStreak > Data.BestStreak) Data.BestStreak = Data.CurrentStreak;
            session.NotifyExternalStateChanged();
        }

        private int GetRewardDay()
        {
            return Math.Max(1, (Math.Max(1, Data.CurrentStreak) - 1) % CoinRewards.Length + 1);
        }

        private static string TodayKey()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }
}
