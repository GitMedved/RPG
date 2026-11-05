using System.Collections.Generic;
using MiaMax.Infrastructure.Save;
using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// M46: lightweight wind animation and optional ambient audio for the main backyard.
    /// Decorative transforms are discovered once and cached; no gameplay objects are moved.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class BackyardAmbientPrototype : MonoBehaviour
    {
        private readonly List<Transform> foliage = new List<Transform>();
        private readonly List<Quaternion> baseRotations = new List<Quaternion>();
        private readonly List<float> phases = new List<float>();
        private readonly List<float> strengths = new List<float>();
        private AudioSource windSource;
        private AudioSource birdsSource;
        private bool reducedMotion;

        public static void Attach()
        {
            if (FindFirstObjectByType<BackyardAmbientPrototype>() != null) return;
            GameObject root = new GameObject("M46_Backyard_Ambience");
            root.AddComponent<BackyardAmbientPrototype>();
        }

        private void Awake()
        {
            CacheFoliage();
            ConfigureAmbientAudio();
        }

        private void Update()
        {
            reducedMotion = GameSessionBootstrap.Current != null &&
                GameSessionBootstrap.Current.Data.Settings != null &&
                GameSessionBootstrap.Current.Data.Settings.ReducedMotion;

            float time = Time.time;
            for (int i = 0; i < foliage.Count; i++)
            {
                Transform part = foliage[i];
                if (part == null) continue;

                if (reducedMotion)
                {
                    part.localRotation = baseRotations[i];
                    continue;
                }

                float breeze = Mathf.Sin(time * 0.48f + phases[i]) * strengths[i];
                float flutter = Mathf.Sin(time * 0.77f + phases[i] * 1.31f) * strengths[i] * 0.28f;
                part.localRotation = baseRotations[i] * Quaternion.Euler(breeze, 0f, flutter);
            }
        }

        private void CacheFoliage()
        {
            Transform[] all = FindObjectsByType<Transform>(FindObjectsSortMode.None);
            for (int i = 0; i < all.Length; i++)
            {
                Transform part = all[i];
                string objectName = part.name;
                float strength;
                if (objectName.StartsWith("Tree_Canopy_")) strength = 1.25f;
                else if (objectName == "Main_Tree_Canopy") strength = 0.85f;
                else if (objectName == "Garden_Shrub") strength = 1.7f;
                else if (objectName.StartsWith("Flower_Stem_Detail")) strength = 2.3f;
                else if (objectName.StartsWith("Flower_Leaf_Detail")) strength = 3.1f;
                else if (objectName.StartsWith("Flower_Bloom_Detail")) strength = 1.9f;
                else if (objectName.StartsWith("Patio_Planter_") &&
                    (objectName.Contains("_Stem_") || objectName.Contains("_Leaf_") || objectName.Contains("_Bloom_")))
                    strength = objectName.Contains("_Bloom_") ? 2.2f : 2.8f;
                else continue;

                foliage.Add(part);
                baseRotations.Add(part.localRotation);
                phases.Add((i * 2.3999632f) % (Mathf.PI * 2f));
                strengths.Add(strength);
            }
        }

        private void ConfigureAmbientAudio()
        {
            AudioClip wind = Resources.Load<AudioClip>("MiaMax/Audio/Ambience/backyard_wind");
            AudioClip birds = Resources.Load<AudioClip>("MiaMax/Audio/Ambience/backyard_birds");

            if (wind != null)
            {
                windSource = gameObject.AddComponent<AudioSource>();
                windSource.clip = wind;
                windSource.loop = true;
                windSource.playOnAwake = false;
                windSource.spatialBlend = 0f;
                windSource.volume = 0.16f;
                windSource.Play();
            }

            if (birds != null)
            {
                birdsSource = gameObject.AddComponent<AudioSource>();
                birdsSource.clip = birds;
                birdsSource.loop = true;
                birdsSource.playOnAwake = false;
                birdsSource.spatialBlend = 0f;
                birdsSource.volume = 0.10f;
                birdsSource.Play();
            }
        }

        private void OnDisable()
        {
            if (windSource != null) windSource.Stop();
            if (birdsSource != null) birdsSource.Stop();
        }

        private void OnEnable()
        {
            if (windSource != null && windSource.clip != null) windSource.Play();
            if (birdsSource != null && birdsSource.clip != null) birdsSource.Play();
        }
    }
}