using System;
using MiaMax.Domain.Production;
using MiaMax.Domain.Progression;
using UnityEngine;

namespace MiaMax.Domain
{
    /// <summary>Single-slot production with optional authorable catalog definitions.</summary>
    public sealed class ProductionService
    {
        public const string CarrotSoup = "recipe_carrot_soup";
        public const string StrawberryJuice = "recipe_strawberry_juice";
        public const string WoodenToy = "recipe_wooden_toy";

        private readonly GameSession session;
        private readonly BackyardContentCatalog catalog;

        public ProductionService(GameSession session) : this(session,
            Resources.Load<BackyardContentCatalog>("MiaMax/Content/BackyardContentCatalog")) { }

        public ProductionService(GameSession session, BackyardContentCatalog catalog)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            this.catalog = catalog;
        }

        public ProductionJobData ActiveJob => session.Data.ProductionJob;

        public bool TryStart(string recipeId)
        {
            if (ActiveJob != null && ActiveJob.IsActive) return false;
            string input;
            int quantity;
            int seconds;
            if (!TryResolveRecipe(recipeId, out input, out quantity, out seconds)) return false;
            var job = new ProductionJobData
            {
                RecipeId = recipeId,
                StartedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                DurationSeconds = new UpgradeService(session).AdjustProductionDuration(seconds)
            };
            return session.TryStartProduction(input, quantity, job);
        }

        public int RemainingSeconds
        {
            get
            {
                ProductionJobData job = ActiveJob;
                if (job == null || !job.IsActive) return 0;
                long elapsed = Math.Max(0, DateTimeOffset.UtcNow.ToUnixTimeSeconds() - job.StartedAtUnix);
                return (int)Math.Max(0, job.DurationSeconds - elapsed);
            }
        }

        public bool TryCollect()
        {
            ProductionJobData job = ActiveJob;
            if (job == null || !job.IsActive || RemainingSeconds > 0) return false;

            string output;
            int outputQuantity;
            int experience;
            if (!TryResolveOutput(job.RecipeId, out output, out outputQuantity, out experience) ||
                !session.TryCompleteProduction(job, output, outputQuantity, experience)) return false;
            new DailyTaskService(session).RecordCraft();
            new AchievementService(session).RecordCraft();
            new TutorialService(session).TryAdvance(TutorialService.Craft);
            session.NotifyExternalStateChanged();
            return true;
        }

        private bool TryResolveRecipe(string id, out string input, out int quantity, out int seconds)
        {
            if (catalog != null && catalog.TryGet(id, BackyardContentKind.Recipe, out BackyardContentEntry entry))
            {
                input = entry.RequiredItemId;
                quantity = entry.RequiredQuantity;
                seconds = entry.DurationSeconds > 0 ? entry.DurationSeconds : DefaultDuration(id);
                return IsAreaUnlocked(entry.AreaId) && !string.IsNullOrWhiteSpace(input) && quantity > 0;
            }
            return Recipe(id, out input, out quantity, out seconds);
        }

        private bool IsAreaUnlocked(string areaId)
        {
            return string.IsNullOrWhiteSpace(areaId) || new QuestService(session, catalog).IsAreaUnlocked(areaId);
        }

        private bool TryResolveOutput(string id, out string output, out int quantity, out int experience)
        {
            if (catalog != null && catalog.TryGet(id, BackyardContentKind.Recipe, out BackyardContentEntry entry))
            {
                output = entry.RewardItemId;
                quantity = entry.RewardQuantity;
                experience = entry.RewardExperience;
                return IsAreaUnlocked(entry.AreaId) && !string.IsNullOrWhiteSpace(output) && quantity > 0;
            }

            output = OutputFor(id);
            quantity = 1;
            experience = 5;
            return !string.IsNullOrWhiteSpace(output);
        }

        private static int DefaultDuration(string id)
        {
            switch (id)
            {
                case CarrotSoup: return 180;
                case StrawberryJuice: return 240;
                case WoodenToy: return 300;
                default: return 180;
            }
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
            switch (id)
            {
                case CarrotSoup: return "product_carrot_soup";
                case StrawberryJuice: return "product_strawberry_juice";
                case WoodenToy: return "product_wooden_toy";
                default: return null;
            }
        }
    }
}