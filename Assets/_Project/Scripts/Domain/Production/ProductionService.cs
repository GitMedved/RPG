using System;
using System.Collections.Generic;
using MiaMax.Domain.Production;
namespace MiaMax.Domain
{
    public sealed class ProductionService
    {
        public const string CarrotSoup = "recipe_carrot_soup";
        public const string StrawberryJuice = "recipe_strawberry_juice";
        public const string WoodenToy = "recipe_wooden_toy";
        private readonly GameSession session;
        public ProductionService(GameSession session) { this.session = session ?? throw new ArgumentNullException(nameof(session)); }
        public ProductionJobData ActiveJob => session.Data.ProductionJob;
        public bool TryStart(string recipeId)
        {
            if (ActiveJob != null && ActiveJob.IsActive) return false;
            string input; int quantity; int seconds;
            if (!Recipe(recipeId, out input, out quantity, out seconds)) return false;
            if (!session.TryRemoveItem(input, quantity)) return false;
            session.Data.ProductionJob = new ProductionJobData { RecipeId = recipeId, StartedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), DurationSeconds = seconds };
            session.NotifyExternalStateChanged();
            return true;
        }
        public int RemainingSeconds
        {
            get
            {
                var job = ActiveJob;
                if (job == null || !job.IsActive) return 0;
                long elapsed = Math.Max(0, DateTimeOffset.UtcNow.ToUnixTimeSeconds() - job.StartedAtUnix);
                return (int)Math.Max(0, job.DurationSeconds - elapsed);
            }
        }
        public bool TryCollect()
        {
            var job = ActiveJob;
            if (job == null || !job.IsActive || RemainingSeconds > 0) return false;
            string output = OutputFor(job.RecipeId);
            if (string.IsNullOrEmpty(output) || !session.AddItem(output, 1)) return false;
            session.Data.ProductionJob = new ProductionJobData();
            session.AddExperience(5);
            session.NotifyExternalStateChanged();
            return true;
        }
        public static bool Recipe(string id, out string input, out int quantity, out int seconds)
        {
            input = null; quantity = 0; seconds = 0;
            switch (id)
            {
                case CarrotSoup: input = "crop_carrot"; quantity = 3; seconds = 180; return true;
                case StrawberryJuice: input = "crop_strawberry"; quantity = 3; seconds = 240; return true;
                case WoodenToy: input = "wood"; quantity = 2; seconds = 300; return true;
                default: return false;
            }
        }
        public static string OutputFor(string id)
        {
            switch (id) { case CarrotSoup: return "product_carrot_soup"; case StrawberryJuice: return "product_strawberry_juice"; case WoodenToy: return "product_wooden_toy"; default: return null; }
        }
    }
}