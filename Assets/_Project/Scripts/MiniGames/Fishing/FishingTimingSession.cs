using System;

namespace MiaMax.Domain.MiniGames
{
    public sealed class FishingTimingSession
    {
        public bool IsActive { get; private set; } = true;
        public bool IsCaught { get; private set; }
        public int Quality { get; private set; }
        public float TargetCenter { get; }
        public float TargetWidth { get; }
        public float Progress { get; private set; }

        public FishingTimingSession(int seed = 0, float targetWidth = 0.22f)
        {
            var random = seed == 0 ? new Random() : new Random(seed);
            TargetWidth = Math.Max(0.08f, Math.Min(0.45f, targetWidth));
            TargetCenter = (float)(0.2 + random.NextDouble() * 0.6);
        }

        public bool Tick(float deltaSeconds, float reelInput)
        {
            if (!IsActive) return false;
            float input = Math.Max(0f, Math.Min(1f, reelInput));
            float drift = (input - 0.5f) * 0.65f;
            Progress = Math.Max(0f, Math.Min(1f, Progress + drift * Math.Max(0f, deltaSeconds)));
            if (Progress <= 0f || Progress >= 1f) IsActive = false;
            return IsActive;
        }

        public bool TryFinish(float timingPosition)
        {
            if (!IsActive) return false;
            float distance = Math.Abs(timingPosition - TargetCenter);
            float normalized = distance / (TargetWidth * 0.5f);
            if (normalized <= 1f)
            {
                IsCaught = true;
                Quality = normalized <= 0.35f ? 3 : normalized <= 0.7f ? 2 : 1;
            }
            IsActive = false;
            return IsCaught;
        }
    }
}
