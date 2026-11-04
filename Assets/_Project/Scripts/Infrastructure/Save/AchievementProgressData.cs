using System;

namespace MiaMax.Infrastructure.Save
{
    [Serializable]
    public sealed class AchievementProgressData
    {
        public string AchievementId;
        public int Progress;
        public bool RewardClaimed;
    }
}
