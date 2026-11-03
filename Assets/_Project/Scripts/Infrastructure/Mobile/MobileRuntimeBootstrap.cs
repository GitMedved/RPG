using UnityEngine;

namespace MiaMax.Infrastructure.Mobile
{
    /// <summary>
    /// Conservative mobile runtime defaults. Does not alter quality-level assets or
    /// force a resolution; project-specific URP quality tiers remain configured in Editor.
    /// </summary>
    public sealed class MobileRuntimeBootstrap : MonoBehaviour
    {
        [SerializeField, Range(30, 120)] private int targetFrameRate = 60;
        [SerializeField] private bool keepScreenAwake = true;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Create()
        {
            if (FindFirstObjectByType<MobileRuntimeBootstrap>() != null) return;
            new GameObject("Mobile Runtime Bootstrap").AddComponent<MobileRuntimeBootstrap>();
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = Mathf.Clamp(targetFrameRate, 30, 120);
            QualitySettings.vSyncCount = 0;
            if (keepScreenAwake) Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused) return;
            // Reassert only runtime pacing after returning from background.
            Application.targetFrameRate = Mathf.Clamp(targetFrameRate, 30, 120);
        }
    }
}
