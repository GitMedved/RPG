using System;
using System.Collections.Generic;
using System.Linq;
using MiaMax.Domain;
using MiaMax.Infrastructure.Save;

namespace MiaMax.Domain.MiniGames
{
    /// <summary>Single trusted reward entry point for offline mini-game results.</summary>
    public sealed class MiniGameRewardService
    {
        public const string Match3 = "match3";
        public const string Fishing = "fishing";
        public const string MaxRun = "max_run";

        private readonly GameSession session;
        private static readonly string[] AllowedGames = { Match3, Fishing, MaxRun };

        public MiniGameRewardService(GameSession session)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            if (session.Data.MiniGameRewardClaims == null)
                session.Data.MiniGameRewardClaims = new List<MiniGameRewardClaimData>();
        }

        public bool TryClaim(string runId, string gameId, int score, out int coins, out int experience)
        {
            coins = 0;
            experience = 0;
            if (string.IsNullOrWhiteSpace(runId) || runId.Length > 80 ||
                !AllowedGames.Contains(gameId) || score < 0 || score > 1000000)
                return false;

            if (session.Data.MiniGameRewardClaims.Any(x => x != null && x.RunId == runId))
                return false;

            // A fixed, score-banded reward prevents unbounded currency farming.
            int band = score >= 1000 ? 3 : score >= 500 ? 2 : score >= 150 ? 1 : 0;
            coins = 3 + band * 2;
            experience = 2 + band * 2;

            session.Data.MiniGameRewardClaims.Add(new MiniGameRewardClaimData
            {
                RunId = runId,
                GameId = gameId,
                ClaimedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            });

            if (!session.TryAddCoins(coins) || !session.AddExperience(experience))
            {
                session.Data.MiniGameRewardClaims.RemoveAll(x => x != null && x.RunId == runId);
                coins = 0;
                experience = 0;
                session.NotifyExternalStateChanged();
                return false;
            }

            // Bound local history while retaining enough IDs for practical replay protection.
            if (session.Data.MiniGameRewardClaims.Count > 500)
                session.Data.MiniGameRewardClaims.RemoveAt(0);
            session.NotifyExternalStateChanged();
            return true;
        }
    }
}
