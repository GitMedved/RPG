using System;

namespace MiaMax.Domain.MiniGames
{
    [Serializable]
    public sealed class MiniGameRewardClaimData
    {
        public string RunId;
        public string GameId;
        public long ClaimedAtUnix;
    }
}
