using System;
namespace MiaMax.Domain.Production
{
    [Serializable]
    public sealed class ProductionJobData
    {
        public string RecipeId;
        public long StartedAtUnix;
        public int DurationSeconds;
        public bool IsActive => !string.IsNullOrEmpty(RecipeId) && StartedAtUnix > 0;
    }
}