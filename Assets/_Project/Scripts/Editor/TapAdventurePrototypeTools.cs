#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// Small, safe QA helpers for the standalone tap-adventure prototype.
    /// These commands never modify scenes, assets or project settings.
    /// </summary>
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

        private static readonly string[] RequiredMaterials =
        {
            "Grass_Lawn",
            "Wood_Warm",
            "Plaster_Cream",
            "Stone_Patio",
            "Leaf_Green",
            "Flower_Petal",
            "Roof_Shingle",
            "Water_Pond"
        };

        [MenuItem("Mia & Max/Prototype/Reset Tap Adventure Progress")]
        private static void ResetTapAdventureProgress()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Reset Tap Adventure Progress",
                "Delete only Mia & Max tap-adventure prototype progress on this device? Other game saves and PlayerPrefs will be preserved.",
                "Reset Prototype Progress",
                "Cancel");

            if (!confirmed) return;

            foreach (string key in ProgressKeys)
                PlayerPrefs.DeleteKey(key);

            PlayerPrefs.Save();
            Debug.Log("[Mia & Max QA] Tap Adventure prototype progress reset. Restart Play Mode to begin a fresh story.");
        }

        [MenuItem("Mia & Max/Prototype/Validate Tap Adventure Materials")]
        private static void ValidateTapAdventureMaterials()
        {
            int found = 0;
            foreach (string materialName in RequiredMaterials)
            {
                Material material = Resources.Load<Material>("MiaMax/Materials/" + materialName);
                if (material != null)
                {
                    found++;
                    Debug.Log("[Mia & Max QA] Material OK: " + materialName);
                }
                else
                {
                    Debug.LogError("[Mia & Max QA] Missing runtime material: Resources/MiaMax/Materials/" + materialName);
                }
            }

            Debug.Log("[Mia & Max QA] Runtime material validation: " + found + "/" + RequiredMaterials.Length + " found.");
        }
    }
}
#endif
