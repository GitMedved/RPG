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
            if (!Current.Data.StarterKitGranted)
            {
                // Grant the starter kit only once. An empty inventory later in the game
                // is a legitimate player state and must never recreate free resources.
                if (Current.Data.Inventory.Count == 0)
                {
                    Current.AddItem("seed_carrot", 3);
                    Current.AddItem("seed_tomato", 3);
                    Current.AddItem("seed_strawberry", 3);
                    Current.AddItem("wood", 2);
                }

                Current.Data.StarterKitGranted = true;
            }

            Current.StateChanged += SaveNow;
            SaveNow();
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
