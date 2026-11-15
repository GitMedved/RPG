using System.Collections.Generic;
using UnityEngine;

namespace MiaMax.Infrastructure.Audio
{
    public enum GameAudioCue
    {
        UiTap, Plant, Harvest, CraftStart, CraftReady, Reward, Collectible,
        Interact, MiniGameStart, MiniGameWin, MiniGameMiss, MaxMeow
    }

    /// <summary>
    /// Optional audio facade. Assign licensed clips in the Inspector; missing clips are
    /// intentionally silent and never interrupt gameplay. Voice lines use a separate source.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GameAudioService : MonoBehaviour
    {
        public static GameAudioService Instance { get; private set; }

        [Header("Sources")]
        [SerializeField] private AudioSource effectsSource;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource voiceSource;
        [SerializeField] private AudioSource ambientWindSource;
        [SerializeField] private AudioSource ambientBirdsSource;

        [Header("Sound effects")]
        [SerializeField] private AudioClip uiTap;
        [SerializeField] private AudioClip plant;
        [SerializeField] private AudioClip harvest;
        [SerializeField] private AudioClip craftStart;
        [SerializeField] private AudioClip craftReady;
        [SerializeField] private AudioClip reward;
        [SerializeField] private AudioClip collectible;
        [SerializeField] private AudioClip interact;
        [SerializeField] private AudioClip miniGameStart;
        [SerializeField] private AudioClip miniGameWin;
        [SerializeField] private AudioClip miniGameMiss;
        [SerializeField] private AudioClip maxMeow1;
        [SerializeField] private AudioClip maxMeow2;
        private float nextMaxMeowTime;
        private bool alternateMeow;

        private readonly HashSet<string> missingVoiceLineWarnings = new HashSet<string>();

        private float effectsMix = 1f;
        private float musicMix = 1f;
        private float voiceMix = 1f;
        private float voiceBaseVolume = 1f;
        private float musicBaseVolume = 0.22f;
        private float ambientWindBaseVolume = 0.13f;
        private float ambientBirdsBaseVolume = 0.10f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureExists()
        {
            if (Instance != null) return;
            GameObject audioObject = new GameObject("Game Audio Service");
            audioObject.AddComponent<GameAudioService>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            if (effectsSource == null) effectsSource = gameObject.AddComponent<AudioSource>();
            if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();
            if (voiceSource == null) voiceSource = gameObject.AddComponent<AudioSource>();
            if (ambientWindSource == null) ambientWindSource = gameObject.AddComponent<AudioSource>();
            if (ambientBirdsSource == null) ambientBirdsSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            effectsSource.playOnAwake = false;
            voiceSource.playOnAwake = false;
            musicSource.playOnAwake = false;
            ambientWindSource.playOnAwake = false;
            ambientBirdsSource.playOnAwake = false;
            ambientWindSource.loop = true;
            ambientBirdsSource.loop = true;
            LoadResourceClips();
            InstallPrototypeFeedbackSounds();
            SetMusicById("backyard_theme", 0.22f);
        }

        private void LoadResourceClips()
        {
            uiTap = LoadIfMissing(uiTap, "MiaMax/Audio/SFX/ui_tap");
            plant = LoadIfMissing(plant, "MiaMax/Audio/SFX/plant");
            harvest = LoadIfMissing(harvest, "MiaMax/Audio/SFX/harvest");
            craftStart = LoadIfMissing(craftStart, "MiaMax/Audio/SFX/craft_start");
            craftReady = LoadIfMissing(craftReady, "MiaMax/Audio/SFX/craft_ready");
            reward = LoadIfMissing(reward, "MiaMax/Audio/SFX/reward");
            collectible = LoadIfMissing(collectible, "MiaMax/Audio/SFX/collectible");
            interact = LoadIfMissing(interact, "MiaMax/Audio/SFX/interact");
            miniGameStart = LoadIfMissing(miniGameStart, "MiaMax/Audio/SFX/minigame_start");
            miniGameWin = LoadIfMissing(miniGameWin, "MiaMax/Audio/SFX/minigame_win");
            miniGameMiss = LoadIfMissing(miniGameMiss, "MiaMax/Audio/SFX/minigame_miss");
            maxMeow1 = LoadIfMissing(maxMeow1, "MiaMax/Audio/SFX/max_meow_01");
            maxMeow2 = LoadIfMissing(maxMeow2, "MiaMax/Audio/SFX/max_meow_02");
        }

        // Self-contained fallback: small original feedback cues, only when the
        // optional production WAV assets have not yet been imported.
        private void InstallPrototypeFeedbackSounds()
        {
            if (uiTap == null) uiTap = MakeTone("MVP Tap", 520f, 0.07f);
            if (plant == null) plant = MakeTone("MVP Water", 390f, 0.18f);
            if (harvest == null) harvest = MakeTone("MVP Harvest", 660f, 0.16f);
            if (craftStart == null) craftStart = MakeTone("MVP Craft", 410f, 0.15f);
            if (craftReady == null) craftReady = MakeTone("MVP Craft Ready", 740f, 0.22f);
            if (reward == null) reward = MakeTone("MVP Reward", 880f, 0.23f);
            if (collectible == null) collectible = MakeTone("MVP Collect", 710f, 0.11f);
            if (interact == null) interact = MakeTone("MVP Interact", 460f, 0.09f);
            if (miniGameStart == null) miniGameStart = MakeTone("MVP Mini-game", 550f, 0.16f);
            if (miniGameWin == null) miniGameWin = MakeTone("MVP Success", 990f, 0.30f);
            if (miniGameMiss == null) miniGameMiss = MakeTone("MVP Retry", 310f, 0.15f);
        }

        private static AudioClip MakeTone(string clipName, float frequency, float duration)
        {
            const int sampleRate = 22050;
            int count = Mathf.Max(1, Mathf.CeilToInt(duration * sampleRate));
            float[] samples = new float[count];
            for (int i = 0; i < count; i++)
            {
                float time = (float)i / sampleRate;
                float progress = (float)i / count;
                float envelope = Mathf.Sin(progress * Mathf.PI);
                envelope *= envelope;
                float chirp = frequency * (1f + 0.12f * progress);
                samples[i] = 0.13f * envelope * Mathf.Sin(2f * Mathf.PI * chirp * time);
            }
            AudioClip clip = AudioClip.Create(clipName, count, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private static AudioClip LoadIfMissing(AudioClip current, string resourcePath)
        {
            return current != null ? current : Resources.Load<AudioClip>(resourcePath);
        }

        private void LateUpdate()
        {
            // Voice priority: lower the bed gently while Mia or Max talks,
            // then restore it without abrupt volume jumps.
            bool speaking = voiceSource != null && voiceSource.isPlaying;
            float duck = speaking ? 0.34f : 1f;
            float step = Mathf.Max(.01f, Time.unscaledDeltaTime) * (speaking ? 2.0f : 1.1f);
            if (musicSource != null)
                musicSource.volume = Mathf.MoveTowards(musicSource.volume,
                    musicBaseVolume * musicMix * duck, step);
            if (ambientWindSource != null)
                ambientWindSource.volume = Mathf.MoveTowards(ambientWindSource.volume,
                    ambientWindBaseVolume * effectsMix * duck, step);
            if (ambientBirdsSource != null)
                ambientBirdsSource.volume = Mathf.MoveTowards(ambientBirdsSource.volume,
                    ambientBirdsBaseVolume * effectsMix * duck, step);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void Play(GameAudioCue cue, float volume = 1f)
        {
            if (effectsSource == null) return;
            AudioClip clip = GetClip(cue);
            if (clip != null) effectsSource.PlayOneShot(clip, Mathf.Clamp01(volume));
        }

        public void PlayMaxMeow()
        {
            if (effectsSource == null || Time.unscaledTime < nextMaxMeowTime) return;
            if (voiceSource != null && voiceSource.isPlaying) return;
            AudioClip clip = alternateMeow ? maxMeow2 : maxMeow1;
            if (clip == null) clip = maxMeow1 != null ? maxMeow1 : maxMeow2;
            if (clip == null) return;
            alternateMeow = !alternateMeow;
            nextMaxMeowTime = Time.unscaledTime + 4.5f;
            effectsSource.PlayOneShot(clip, 0.42f);
        }

        public void PlayVoiceLine(AudioClip clip, float volume = 1f)
        {
            if (voiceSource == null || clip == null) return;
            voiceSource.Stop();
            voiceSource.clip = clip;
            voiceBaseVolume = Mathf.Clamp01(volume);
            voiceSource.volume = voiceBaseVolume * voiceMix;
            voiceSource.loop = false;
            voiceSource.Play();
        }

        /// <summary>
        /// Loads and plays a named voice take from Resources/MiaMax/Audio/Voice.
        /// Use stable IDs from M49_VOICE_TEST_SCRIPT.md (for example mia_greeting_01).
        /// </summary>
        public bool PlayVoiceLine(string lineId, float volume = 1f)
        {
            if (voiceSource == null || string.IsNullOrEmpty(lineId)) return false;
            string safeId = lineId.Trim();
            if (safeId.Contains("/") || safeId.Contains("\\") || safeId.Contains("..")) return false;

            AudioClip clip = Resources.Load<AudioClip>("MiaMax/Audio/Voice/" + safeId);
            if (clip == null)
            {
                // Voice takes are optional during prototype development. Report each
                // missing take once instead of flooding the Console on repeated actions.
                if (missingVoiceLineWarnings.Add(safeId))
                    Debug.LogWarning("[MiaMax Audio] Optional voice line is not installed: " + safeId +
                        ". Expected Resources/MiaMax/Audio/Voice/" + safeId + ".wav");
                return false;
            }

            PlayVoiceLine(clip, volume);
            return true;
        }

        public bool SetMusicById(string musicId, float volume = 0.22f)
        {
            if (string.IsNullOrEmpty(musicId) || musicId.Contains("/") || musicId.Contains("\\") || musicId.Contains(".."))
                return false;
            AudioClip clip = Resources.Load<AudioClip>("MiaMax/Audio/Music/" + musicId);
            if (clip == null) return false;
            SetMusic(clip, volume);
            return true;
        }

        public void SetAdventureAmbience(string windId, string birdsId,
            float windVolume = 0.13f, float birdsVolume = 0.10f)
        {
            ambientWindBaseVolume = Mathf.Clamp01(windVolume);
            ambientBirdsBaseVolume = Mathf.Clamp01(birdsVolume);
            ConfigureLoop(ambientWindSource, windId, ambientWindBaseVolume * effectsMix);
            ConfigureLoop(ambientBirdsSource, birdsId, ambientBirdsBaseVolume * effectsMix);
        }

        private void ConfigureLoop(AudioSource source, string resourceId, float volume)
        {
            if (source == null) return;
            AudioClip clip = string.IsNullOrEmpty(resourceId)
                ? null
                : Resources.Load<AudioClip>("MiaMax/Audio/Ambience/" + resourceId);
            if (source.clip == clip && clip != null && source.isPlaying)
            {
                source.volume = Mathf.Clamp01(volume);
                return;
            }
            source.Stop();
            source.clip = clip;
            source.volume = Mathf.Clamp01(volume);
            if (clip != null) source.Play();
        }

        public void SetMusic(AudioClip clip, float volume = 0.35f)
        {
            if (musicSource == null) return;
            musicBaseVolume = Mathf.Clamp01(volume);
            if (musicSource.clip == clip && musicSource.isPlaying)
            {
                musicSource.volume = musicBaseVolume * musicMix;
                return;
            }
            musicSource.Stop();
            musicSource.clip = clip;
            musicSource.volume = musicBaseVolume * musicMix;
            if (clip != null) musicSource.Play();
        }

        public void SetMix(float effects, float music, float voice)
        {
            effectsMix = Mathf.Clamp01(effects);
            musicMix = Mathf.Clamp01(music);
            voiceMix = Mathf.Clamp01(voice);
            if (effectsSource != null) effectsSource.volume = effectsMix;
            if (musicSource != null) musicSource.volume = musicBaseVolume * musicMix;
            if (voiceSource != null && voiceSource.isPlaying) voiceSource.volume = voiceBaseVolume * voiceMix;
            if (ambientWindSource != null) ambientWindSource.volume = ambientWindBaseVolume * effectsMix;
            if (ambientBirdsSource != null) ambientBirdsSource.volume = ambientBirdsBaseVolume * effectsMix;
        }

        public void StopVoice() { if (voiceSource != null) voiceSource.Stop(); }

        private AudioClip GetClip(GameAudioCue cue)
        {
            switch (cue)
            {
                case GameAudioCue.UiTap: return uiTap;
                case GameAudioCue.Plant: return plant;
                case GameAudioCue.Harvest: return harvest;
                case GameAudioCue.CraftStart: return craftStart;
                case GameAudioCue.CraftReady: return craftReady;
                case GameAudioCue.Reward: return reward;
                case GameAudioCue.Collectible: return collectible;
                case GameAudioCue.Interact: return interact;
                case GameAudioCue.MiniGameStart: return miniGameStart;
                case GameAudioCue.MiniGameWin: return miniGameWin;
                case GameAudioCue.MiniGameMiss: return miniGameMiss;
                case GameAudioCue.MaxMeow: return alternateMeow ? maxMeow2 : maxMeow1;
                default: return null;
            }
        }
    }
}
