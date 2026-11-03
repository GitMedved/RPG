using System;
using System.Linq;
using MiaMax.Domain.Farming;
using MiaMax.Domain.MiniGames;
using MiaMax.Infrastructure.Save;

namespace MiaMax.Domain.Progression
{
    /// <summary>Persistent, lightweight first-session guide driven by real player progress.</summary>
    public sealed class TutorialService
    {
        private readonly GameSession session;
        private readonly FarmingService farming;

        private static readonly string[] Objectives =
        {
            "Move Mia around the backyard",
            "Plant your first seed",
            "Harvest your first crop",
            "Make and collect a workshop product",
            "Deliver your first neighbour order",
            "Play an arcade mini-game"
        };

        public TutorialService(GameSession session)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            farming = new FarmingService(session);
            session.Data.TutorialStep = Math.Max(0, Math.Min(Objectives.Length, session.Data.TutorialStep));
            if (session.Data.TutorialCompleted) session.Data.TutorialStep = Objectives.Length;
        }

        public int Step => session.Data.TutorialStep;
        public bool IsComplete => Step >= Objectives.Length;
        public string CurrentObjective => IsComplete ? "You are ready to explore on your own!" : Objectives[Step];
        public string GetStatus()
        {
            RefreshProgress();
            return IsComplete ? "Tutorial complete — enjoy the backyard!" :
                "FIRST ADVENTURE  " + (Step + 1) + "/" + Objectives.Length + ": " + CurrentObjective;
        }

        public void MarkMovementSeen()
        {
            if (session.Data.TutorialMovementSeen) return;
            session.Data.TutorialMovementSeen = true;
            RefreshProgress();
            session.NotifyExternalStateChanged();
        }

        public void RefreshProgress()
        {
            int previous = session.Data.TutorialStep;
            while (session.Data.TutorialStep < Objectives.Length && IsObjectiveComplete(session.Data.TutorialStep))
                session.Data.TutorialStep++;

            session.Data.TutorialCompleted = session.Data.TutorialStep >= Objectives.Length;
            if (previous != session.Data.TutorialStep)
                session.NotifyExternalStateChanged();
        }

        private bool IsObjectiveComplete(int step)
        {
            switch (step)
            {
                case 0: return session.Data.TutorialMovementSeen;
                case 1: return session.Data.FarmPlots != null && session.Data.FarmPlots.Any(x => x != null && x.IsPlanted);
                case 2: return session.Data.Inventory != null && session.Data.Inventory.Any(x => x != null &&
                    (x.ItemId == FarmingService.Carrot || x.ItemId == FarmingService.Tomato || x.ItemId == FarmingService.Strawberry));
                case 3: return session.Data.Inventory != null && session.Data.Inventory.Any(x => x != null &&
                    (x.ItemId == "product_carrot_soup" || x.ItemId == "product_strawberry_juice" || x.ItemId == "product_wooden_toy"));
                case 4: return session.Data.CompletedOrders != null && session.Data.CompletedOrders.Any(x => x != null);
                case 5: return session.Data.MiniGameRewardClaims != null && session.Data.MiniGameRewardClaims.Any(x => x != null);
                default: return false;
            }
        }
    }
}
