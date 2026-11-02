using UnityEngine;
using MiaMax.Domain;

namespace MiaMax.Infrastructure.Save
{
    [DefaultExecutionOrder(-100)]
    public sealed class GameSessionBootstrap : MonoBehaviour
    {
        public static GameSession Current { get; private set; }
        private LocalSaveService saveService;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Create()
        {
            if (FindObjectOfType<GameSessionBootstrap>() != null) return;
            new GameObject("Game Session").AddComponent<GameSessionBootstrap>();
        }

        private void Awake()
        {
            if (Current != null && Current != null)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            saveService = new LocalSaveService();
            Current = new GameSession(saveService.LoadOrCreate());
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
    }
}
