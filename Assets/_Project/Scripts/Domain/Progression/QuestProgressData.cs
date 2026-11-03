using System;

namespace MiaMax.Domain.Progression
{
    [Serializable]
    public sealed class QuestProgressData
    {
        public string QuestId;
        public long ClaimedAtUnix;
    }
}
