using UnityEngine;
using MiaMax.Domain;

namespace MiaMax.Infrastructure.Save
{
    [DefaultExecutionOrder(-100)]
    public sealed class GameSessionBootstrap : MonoBehaviour
    {
        public static GameSession Current { get; private set; }
        private static GameSessionBootstrap instance;
        private LocalSaveService saveService;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Create()
        {
            if (FindObjectOfType<GameSessionBootstrap>() != null) return;
            new GameObject("Game Session").AddComponent<GameSessionBootstrap>();
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            saveService = new LocalSaveService();
            Current = new GameSession(saveService.LoadOrCreate());
            Current.StateChanged += SaveNow;
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused) SaveNow();
        }

        private void OnApplicationQuit()
        {
            SaveNow();
        }

        public void SaveNow()
        {
            if (saveService != null && Current != null)
                saveService.Save(Current.Data);
        }

        private void OnDestroy()
        {
            if (Current != null) Current.StateChanged -= SaveNow;
            if (instance == this)
            {
                instance = null;
                Current = null;
            }
        }
    }
}
