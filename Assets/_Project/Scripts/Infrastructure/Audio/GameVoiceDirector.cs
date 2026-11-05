using System.Collections.Generic;
using UnityEngine;

namespace MiaMax.Infrastructure.Audio
{
    /// <summary>
    /// Routes successful gameplay outcomes to optional M49 voice assets.
    /// Missing clips stay silent; repeated cues are rate-limited.
    /// </summary>
    public static class GameVoiceDirector
    {
        private static readonly Dictionary<string, float> LastPlayedAt = new Dictionary<string, float>();
        private const float SameCueCooldown = 2.5f;

        public static bool PlayMia(string lineId) { return TryPlay(lineId); }
        public static bool PlayMax(string lineId) { return TryPlay(lineId); }

        private static bool TryPlay(string lineId)
        {
            if (string.IsNullOrEmpty(lineId) || GameAudioService.Instance == null) return false;
            float now = Time.unscaledTime;
            float previous;
            if (LastPlayedAt.TryGetValue(lineId, out previous) && now - previous < SameCueCooldown)
                return false;
            if (!GameAudioService.Instance.PlayVoiceLine(lineId)) return false;
            LastPlayedAt[lineId] = now;
            return true;
        }
    }
}
