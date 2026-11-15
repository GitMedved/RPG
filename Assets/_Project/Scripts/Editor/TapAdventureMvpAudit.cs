#if UNITY_EDITOR
using System.Collections.Generic;
using MiaMax.Gameplay;
using MiaMax.Infrastructure.Save;
using UnityEditor;
using UnityEngine;

namespace MiaMax.EditorTools
{
    /// <summary>
    /// MVP-specific checks for the tap-only application. Run Preflight before Play,
    /// and Inspect Running World during Play. Neither command changes player progress.
    /// </summary>
    public static class TapAdventureMvpAudit
    {
        private static int passed;
        private static int failed;
        private static readonly string[] Materials =
        {
            "Grass_Lawn", "Wood_Warm", "Plaster_Cream", "Stone_Patio",
            "Leaf_Green", "Flower_Petal", "Roof_Shingle", "Water_Pond"
        };

        [MenuItem("Mia & Max/MVP/1. Preflight Assets & Scene")]
        public static void Preflight()
        {
            Begin();
            bool startupScene = false;
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled && scene.path == "Assets/_Project/Scenes/Backyard.unity" &&
                    AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path) != null)
                    startupScene = true;
            }
            Check(startupScene, "Backyard scene exists and is enabled in Build Settings");
            Check(AssetDatabase.LoadAssetAtPath<SceneAsset>(
                "Assets/_Project/Scenes/Backyard.unity") != null, "Backyard scene asset");
            foreach (string name in Materials)
                Check(Resources.Load<Material>("MiaMax/Materials/" + name) != null, "URP material " + name);
            Check(Resources.Load<Texture2D>("MiaMax/Art/MiaPortrait") != null, "Original Mia reference portrait");
            Check(Resources.Load<Texture2D>("MiaMax/Art/MaxPortrait") != null, "Original Max reference portrait");
            Finish("PREFLIGHT");
        }

        [MenuItem("Mia & Max/MVP/2. Inspect Running World")]
        public static void InspectRunningWorld()
        {
            Begin();
            TapAdventureWorldBootstrap mvp =
                Object.FindFirstObjectByType<TapAdventureWorldBootstrap>();
            Check(mvp != null, "Tap Adventure runtime is active");
            Camera camera = Camera.main;
            Check(camera != null && camera.orthographic && camera.transform.position.y > 5f,
                "Angled orthographic camera");
            Check(Object.FindFirstObjectByType<BackyardPrototypeBootstrap>() == null,
                "No legacy free-roam world loaded");
            Check(Object.FindFirstObjectByType<GameSessionBootstrap>() == null,
                "No competing legacy game session");
            var ids = new HashSet<string>();
            foreach (TapAdventureTarget target in
                Object.FindObjectsByType<TapAdventureTarget>(
                    FindObjectsInactive.Include, FindObjectsSortMode.None))
                ids.Add(target.Id);
            foreach (string id in new[]
            {
                "house", "tree", "twigs", "feather", "birdhouse",
                "flowers", "workshop", "pond", "minigame", "gate"
            })
                Check(ids.Contains(id), "Interactable: " + id);
            Check(Object.FindFirstObjectByType<MiaMax.Infrastructure.Audio.GameAudioService>() != null,
                "Audio service with procedural cue fallback");
            Check(Object.FindFirstObjectByType<TapAdventureArtWorld>() != null,
                "Illustrated procedural 3D world");
            Check(Object.FindFirstObjectByType<TapAdventureHud>() != null,
                "Portrait UGUI mobile HUD");
            Check(Object.FindObjectsByType<TapAdventureCharacterRig>(
                FindObjectsSortMode.None).Length == 2,
                "Animated Mia and Max character proxies");
            Check(ids.Contains("gardenstone"), "Secret Garden story marker");
            Finish("RUNNING WORLD");
        }

        [MenuItem("Mia & Max/MVP/2. Inspect Running World", true)]
        private static bool IsPlaying()
        {
            return EditorApplication.isPlaying;
        }

        private static void Begin()
        {
            passed = 0;
            failed = 0;
        }

        private static void Check(bool ready, string description)
        {
            if (ready) { passed++; Debug.Log("[Mia & Max MVP] OK: " + description); }
            else { failed++; Debug.LogError("[Mia & Max MVP] MISSING: " + description); }
        }

        private static void Finish(string title)
        {
            string message = title + ": " + passed + " passed / " + failed + " failed.";
            if (failed == 0) Debug.Log("[Mia & Max MVP] " + message);
            else Debug.LogError("[Mia & Max MVP] " + message);
        }
    }
}
#endif
