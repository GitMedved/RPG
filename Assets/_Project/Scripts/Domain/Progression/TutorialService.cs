using System;
using MiaMax.Domain;

namespace MiaMax.Domain.Progression
{
    /// <summary>
    /// Persistent first-session guide. Gameplay systems advance it only after a real action succeeds.
    /// Steps: move, plant, harvest, craft, deliver, play an arcade game.
    /// </summary>
    public sealed class TutorialService
    {
        public const int Move = 0;
        public const int Plant = 1;
        public const int Harvest = 2;
        public const int Craft = 3;
        public const int Deliver = 4;
        public const int Arcade = 5;
        public const int StepCount = 6;

        private readonly GameSession session;

        public TutorialService(GameSession session)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            if (session.Data.TutorialStep < 0) session.Data.TutorialStep = 0;
            if (session.Data.TutorialStep >= StepCount) Complete();
        }

        public int CurrentStep => session.Data.TutorialStep;
        public bool IsCompleted => session.Data.TutorialCompleted;

        public string CurrentHint
        {
            get
            {
                if (IsCompleted) return "You're ready for your own backyard adventures!";
                switch (CurrentStep)
                {
                    case Move: return "Move Mia with WASD, arrow keys or the touch joystick.";
                    case Plant: return "Visit a garden plot and press E to plant a seed.";
                    case Harvest: return "Wait for a crop to ripen, then press E to harvest it.";
                    case Craft: return "Open the Journal and start a workshop recipe.";
                    case Deliver: return "Collect a finished product and deliver a ready order.";
                    case Arcade: return "Visit the arcade and finish a mini-game.";
                    default: return "Explore the backyard with Mia and Max.";
                }
            }
        }

        public bool TryAdvance(int completedAction)
        {
            if (IsCompleted || completedAction != CurrentStep) return false;
            if (completedAction == Move) session.Data.TutorialMovementSeen = true;
            session.Data.TutorialStep++;
            if (session.Data.TutorialStep >= StepCount) Complete();
            session.NotifyExternalStateChanged();
            return true;
        }

        public void MarkMovementSeen()
        {
            if (IsCompleted || CurrentStep != Move) return;
            session.Data.TutorialMovementSeen = true;
            TryAdvance(Move);
        }

        private void Complete()
        {
            session.Data.TutorialStep = StepCount;
            session.Data.TutorialCompleted = true;
        }
    }
}
