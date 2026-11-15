#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MiaMax.EditorTools
{
    /// <summary>Local MVP progress reset; no other PlayerPrefs keys are touched.</summary>
    public static class TapAdventurePrototypeTools
    {
        private static readonly string[] ProgressKeys =
        {
            "MiaMax.TapAdventure.twigs",
            "MiaMax.TapAdventure.feathers",
            "MiaMax.TapAdventure.flowers",
            "MiaMax.TapAdventure.stars",
            "MiaMax.TapAdventure.birdhouse",
            "MiaMax.TapAdventure.watered",
            "MiaMax.TapAdventure.forest",
            "MiaMax.TapAdventure.treeGathered",
            "MiaMax.TapAdventure.twigsGathered",
            "MiaMax.TapAdventure.featherGathered",
            "MiaMax.TapAdventure.miniGameClaimed",
            "MiaMax.TapAdventure.gardenstoneDiscovered",
            "MiaMax.TapAdventure.flowersReady"
        };

        [MenuItem("Mia & Max/MVP/3. Reset Local Progress")]
        private static void Reset()
        {
            if (!EditorUtility.DisplayDialog("Reset Mia & Max MVP Progress",
                    "Delete only this MVP's saved progress on the local machine? Restart Play Mode after resetting.",
                    "Reset MVP", "Cancel"))
                return;

            foreach (string key in ProgressKeys)
                PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
            Debug.Log("[Mia & Max MVP] Local progress cleared. Restart Play Mode.");
        }
    }
}
#endif
