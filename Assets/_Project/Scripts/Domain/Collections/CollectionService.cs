using System;
using System.Collections.Generic;
using System.Linq;
using MiaMax.Infrastructure.Save;

namespace MiaMax.Domain.Collections
{
    /// <summary>Persistent, one-time discovery and completion rewards for backyard keepsakes.</summary>
    public sealed class CollectionService
    {
        private static readonly string[] KeepSakeIds =
        {
            "keepsake_red_ball", "keepsake_blue_feather", "keepsake_old_button",
            "keepsake_yellow_leaf", "keepsake_tiny_shell"
        };

        private readonly GameSession session;

        public CollectionService(GameSession session)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            if (session.Data.DiscoveredCollectibles == null)
                session.Data.DiscoveredCollectibles = new List<string>();
        }

        public int DiscoveredCount => KeepSakeIds.Count(IsDiscovered);
        public int TotalCount => KeepSakeIds.Length;
        public bool IsComplete => DiscoveredCount == TotalCount;
        public bool IsCompletionRewardClaimed => session.Data.CollectionCompletionRewardClaimed;
        public bool IsDiscovered(string id) =>
            !string.IsNullOrWhiteSpace(id) && session.Data.DiscoveredCollectibles.Contains(id);

        public bool Discover(string id)
        {
            if (!KeepSakeIds.Contains(id) || IsDiscovered(id)) return false;
            session.Data.DiscoveredCollectibles.Add(id);
            session.NotifyExternalStateChanged();
            return true;
        }

        public bool TryClaimCompletionReward()
        {
            if (!IsComplete || session.Data.CollectionCompletionRewardClaimed) return false;
            const int coins = 25, experience = 30;
            if (session.Data.Progress.Coins > int.MaxValue - coins ||
                session.Data.Progress.Experience > int.MaxValue - experience) return false;

            session.Data.CollectionCompletionRewardClaimed = true;
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

        public string GetSummary()
        {
            return "Keepsakes found: " + DiscoveredCount + "/" + TotalCount +
                (IsComplete
                    ? (session.Data.CollectionCompletionRewardClaimed ? ". Collection complete!" : ". Complete! Claim your reward.")
                    : ". Explore the backyard to find them.");
        }
    }
}