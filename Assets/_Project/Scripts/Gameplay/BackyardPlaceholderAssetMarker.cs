using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// Explicitly marks generated backyard art as a temporary blockout asset.
    /// Replace the visual hierarchy with reviewed production art before release.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class BackyardPlaceholderAssetMarker : MonoBehaviour
    {
        [TextArea]
        public string replacementNotes = "Temporary primitive placeholder. Not approved production art.";
        public bool productionApproved;
    }
}