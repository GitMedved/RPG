using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// Compatibility component for older local scenes. All new games start from
    /// TapAdventureWorldBootstrap; the previous joystick/follow-camera world is gone.
    /// This component intentionally has no RuntimeInitializeOnLoadMethod.
    /// </summary>
    [System.Obsolete("Use TapAdventureWorldBootstrap instead.")]
    public sealed class BackyardPrototypeBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            TapAdventureWorldBootstrap.Build();
            enabled = false;
        }
    }
}
