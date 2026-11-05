using System.Collections.Generic;
using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// M45: one lightweight controller per unlocked annex animates selected foliage
    /// and runs a small, local ambient particle effect.
    /// </summary>
    public sealed class AdventureAnnexAtmospherePrototype : MonoBehaviour
    {
        private readonly List<Transform> animatedParts = new List<Transform>();
        private readonly List<Quaternion> baseRotations = new List<Quaternion>();
        private readonly List<float> phases = new List<float>();
        private readonly List<float> amplitudes = new List<float>();
        private bool woodland;
        private bool configured;
        private ParticleSystem ambience;

        public void Configure(bool isWoodland)
        {
            if (configured) return;
            configured = true;
            woodland = isWoodland;
            CacheFoliage();
            CreateAmbientParticles();
        }

        private void Update()
        {
            if (!configured || animatedParts.Count == 0) return;

            float time = Time.time;
            for (int i = 0; i < animatedParts.Count; i++)
            {
                Transform part = animatedParts[i];
                if (part == null) continue;

                float sway = Mathf.Sin(time * (woodland ? 0.62f : 0.82f) + phases[i]) * amplitudes[i];
                float secondary = Mathf.Sin(time * 0.43f + phases[i] * 1.7f) * amplitudes[i] * 0.35f;
                part.localRotation = baseRotations[i] * Quaternion.Euler(sway, 0f, secondary);
            }
        }

        private void CacheFoliage()
        {
            Transform[] all = GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < all.Length; i++)
            {
                Transform part = all[i];
                if (part == transform) continue;

                string partName = part.name;
                bool animate = woodland
                    ? partName == "Woodland_Canopy_Upper" ||
                      partName == "Woodland_Canopy_Side" ||
                      partName == "Woodland_Fern_Frond"
                    : partName == "Garden_Flower_Stem" ||
                      partName == "Garden_Flower_Leaf" ||
                      partName == "Garden_Flower_Bloom";

                if (!animate) continue;
                animatedParts.Add(part);
                baseRotations.Add(part.localRotation);
                phases.Add((i * 2.3999632f) % (Mathf.PI * 2f));
                amplitudes.Add(partName == "Woodland_Fern_Frond" ? 4.0f :
                    partName == "Garden_Flower_Bloom" ? 2.0f :
                    partName == "Garden_Flower_Stem" ? 2.8f :
                    woodland ? 1.15f : 1.6f);
            }
        }

        private void CreateAmbientParticles()
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (shader == null) shader = Shader.Find("Particles/Standard Unlit");
            if (shader == null) return;

            GameObject particleObject = new GameObject(woodland ? "Woodland_Firefly_Drift" : "Garden_Pollen_Drift");
            particleObject.transform.SetParent(transform, false);
            ParticleSystem particles = particleObject.AddComponent<ParticleSystem>();
            ParticleSystem.MainModule main = particles.main;
            main.loop = true;
            main.playOnAwake = false;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            main.maxParticles = woodland ? 16 : 20;
            main.startLifetime = woodland ? 5.5f : 7.0f;
            main.startSpeed = woodland ? 0.025f : 0.045f;
            main.startSize = woodland ? 0.055f : 0.035f;
            main.startColor = woodland
                ? new Color(1f, 0.86f, 0.48f, 0.78f)
                : new Color(1f, 0.94f, 0.70f, 0.62f);
            main.gravityModifier = 0f;
            main.cullingMode = ParticleSystemCullingMode.Automatic;

            ParticleSystem.EmissionModule emission = particles.emission;
            emission.enabled = true;
            emission.rateOverTime = woodland ? 1.1f : 1.5f;

            ParticleSystem.ShapeModule shape = particles.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = woodland ? new Vector3(11f, 3.2f, 10f) : new Vector3(4.5f, 1.8f, 4.5f);
            shape.position = woodland ? new Vector3(-28f, 1.4f, -3.5f) : new Vector3(11.5f, 1.0f, 9.5f);

            ParticleSystem.VelocityOverLifetimeModule velocity = particles.velocityOverLifetime;
            velocity.enabled = true;
            velocity.space = ParticleSystemSimulationSpace.Local;
            velocity.x = new ParticleSystem.MinMaxCurve(-0.025f, 0.025f);
            velocity.y = new ParticleSystem.MinMaxCurve(0.025f, 0.07f);
            velocity.z = new ParticleSystem.MinMaxCurve(-0.018f, 0.018f);

            ParticleSystemRenderer renderer = particleObject.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.material = new Material(shader);
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;

            ambience = particles;
            if (isActiveAndEnabled) ambience.Play();
        }

        private void OnDisable()
        {
            if (ambience != null) ambience.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void OnEnable()
        {
            if (ambience != null && configured) ambience.Play();
        }
    }
}