using System;

namespace MiaMax.Domain.Progression
{
    [Serializable]
    public sealed class PlayerProgressData
    {
        public int Level = 1;
        public int Experience;
        public int Coins;
    }
}
