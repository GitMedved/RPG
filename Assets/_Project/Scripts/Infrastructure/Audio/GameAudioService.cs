using UnityEngine;

namespace MiaMax.Infrastructure.Audio
{
    public enum GameAudioCue
    {
        UiTap, Plant, Harvest, CraftStart, CraftReady, Reward, Collectible,
        Interact, MiniGameStart, MiniGameWin, MiniGameMiss
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
            LoadResourceClips();
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
        }

        private static AudioClip LoadIfMissing(AudioClip current, string resourcePath)
        {
            return current != null ? current : Resources.Load<AudioClip>(resourcePath);
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

        public void PlayVoiceLine(AudioClip clip, float volume = 1f)
        {
            if (voiceSource == null || clip == null) return;
            voiceSource.Stop();
            voiceSource.clip = clip;
            voiceSource.volume = Mathf.Clamp01(volume);
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
                Debug.LogWarning("[MiaMax Audio] Voice line not found: " + safeId +
                    ". Expected Resources/MiaMax/Audio/Voice/" + safeId + ".wav");
                return false;
            }

            PlayVoiceLine(clip, volume);
            return true;
        }

        public bool SetMusicById(string musicId, float volume = 0.22f)
        {
            if (string.IsNullOrEmpty(musicId) || musicId.Contains("/") || musicId.Contains("\\\\") || musicId.Contains(".."))
                return false;
            AudioClip clip = Resources.Load<AudioClip>("MiaMax/Audio/Music/" + musicId);
            if (clip == null) return false;
            SetMusic(clip, volume);
            return true;
        }

        public void SetAdventureAmbience(string windId, string birdsId,
            float windVolume = 0.13f, float birdsVolume = 0.10f)
        {
            ConfigureLoop(ambientWindSource, windId, windVolume);
            ConfigureLoop(ambientBirdsSource, birdsId, birdsVolume);
        }

        private static void ConfigureLoop(AudioSource source, string resourceId, float volume)
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
            if (musicSource.clip == clip && musicSource.isPlaying) return;
            musicSource.Stop();
            musicSource.clip = clip;
            musicSource.volume = Mathf.Clamp01(volume);
            if (clip != null) musicSource.Play();
        }

        public void SetMix(float effects, float music, float voice)
        {
            if (effectsSource != null) effectsSource.volume = Mathf.Clamp01(effects);
            if (musicSource != null) musicSource.volume = Mathf.Clamp01(music);
            if (voiceSource != null) voiceSource.volume = Mathf.Clamp01(voice);
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
                default: return null;
            }
        }
    }
}
