using MiaMax.Domain;
using MiaMax.Infrastructure.Audio;
using MiaMax.Infrastructure.Save;
using UnityEngine;

namespace MiaMax.Infrastructure.Settings
{
    /// <summary>Persistent player preferences with safe defaults for offline play.</summary>
    public sealed class GameSettingsService
    {
        private readonly GameSession session;
        public GameSettingsService(GameSession session)
        {
            this.session = session;
            ApplyAudio();
        }

        public PlayerSettingsData Data => session.Data.Settings;

        public void SetMaster(float value) { Data.MasterVolume = Mathf.Clamp01(value); ApplyAudio(); Save(); }
        public void SetEffects(float value) { Data.EffectsVolume = Mathf.Clamp01(value); ApplyAudio(); Save(); }
        public void SetMusic(float value) { Data.MusicVolume = Mathf.Clamp01(value); ApplyAudio(); Save(); }
        public void SetVoice(float value) { Data.VoiceVolume = Mathf.Clamp01(value); ApplyAudio(); Save(); }
        public void SetUiScale(float value) { Data.UiScale = Mathf.Clamp(value, 0.85f, 1.15f); Save(); }
        public void ToggleVibration() { Data.VibrationEnabled = !Data.VibrationEnabled; Save(); }
        public void ToggleReducedMotion() { Data.ReducedMotion = !Data.ReducedMotion; Save(); }

        public void Vibrate()
        {
            if (Data.VibrationEnabled && Application.isMobilePlatform) Handheld.Vibrate();
        }

        private void ApplyAudio()
        {
            AudioListener.volume = Data.MasterVolume;
            if (GameAudioService.Instance != null)
                GameAudioService.Instance.SetMix(Data.EffectsVolume, Data.MusicVolume, Data.VoiceVolume);
        }

        private void Save()
        {
            session.NotifyExternalStateChanged();
        }
    }
}
