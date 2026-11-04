using System;

namespace MiaMax.Infrastructure.Save
{
    [Serializable]
    public sealed class PlayerSettingsData
    {
        public float MasterVolume = 1f;
        public float EffectsVolume = 1f;
        public float MusicVolume = 0.65f;
        public float VoiceVolume = 1f;
        public float UiScale = 1f;
        public bool VibrationEnabled = true;
        public bool ReducedMotion;
    }
}
