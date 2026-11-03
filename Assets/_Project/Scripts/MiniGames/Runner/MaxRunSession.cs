using System;

namespace MiaMax.Domain.MiniGames
{
    /// <summary>Deterministic, presentation-independent endless-runner session rules.</summary>
    public sealed class MaxRunSession
    {
        public const int LaneCount = 3;
        public int Lane { get; private set; } = 1;
        public float Distance { get; private set; }
        public int CoinsCollected { get; private set; }
        public int ObstaclesAvoided { get; private set; }
        public bool IsFinished { get; private set; }
        public int Score => (int)Math.Floor(Distance) + CoinsCollected * 25 + ObstaclesAvoided * 10;

        public void MoveLeft()
        {
            if (!IsFinished) Lane = Math.Max(0, Lane - 1);
        }

        public void MoveRight()
        {
            if (!IsFinished) Lane = Math.Min(LaneCount - 1, Lane + 1);
        }

        public void Tick(float deltaSeconds, float speed)
        {
            if (IsFinished || deltaSeconds <= 0f || speed <= 0f) return;
            Distance += deltaSeconds * Math.Min(speed, 30f);
        }

        public bool CollectCoin(int lane)
        {
            if (IsFinished || lane != Lane || CoinsCollected >= 10000) return false;
            CoinsCollected++;
            return true;
        }

        public bool AvoidObstacle(int lane)
        {
            if (IsFinished || lane == Lane || ObstaclesAvoided >= 10000) return false;
            ObstaclesAvoided++;
            return true;
        }

        public void HitObstacle(int lane)
        {
            if (!IsFinished && lane == Lane) IsFinished = true;
        }

        public void Finish()
        {
            IsFinished = true;
        }
    }
}
