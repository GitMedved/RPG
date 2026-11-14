using UnityEngine;
using MiaMax.Domain;
using MiaMax.Infrastructure.Save;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// Lightweight one-shot feedback particles for planting, harvesting and workshop actions.
    /// Uses built-in ParticleSystem primitives and a shared unlit material; no external assets required.
    /// </summary>
    public static class BackyardFeedbackVfx
    {
        private static Material sharedMaterial;

        public static void PlayPlant(Vector3 position)
        {
            PlayBurst("VFX_Plant_Sprout", position, new Color(0.52f, 0.82f, 0.31f), 8, 0.85f, 1.15f);
        }

        public static void PlayHarvest(Vector3 position)
        {
            PlayBurst("VFX_Harvest_Sparkle", position, new Color(1f, 0.72f, 0.24f), 18, 1.25f, 1.65f);
        }

        public static void PlayWorkshopStart(Vector3 position)
        {
            PlayBurst("VFX_Workshop_Start", position, new Color(1f, 0.83f, 0.53f), 10, 0.55f, 0.85f);
        }

        public static void PlayWorkshopProgress(Vector3 position)
        {
            PlayBurst("VFX_Workshop_Steam", position, new Color(0.94f, 0.91f, 0.79f, 0.72f), 5, 0.22f, 0.42f);
        }

        public static void PlayWorkshopComplete(Vector3 position)
        {
            PlayBurst("VFX_Workshop_Complete", position, new Color(1f, 0.76f, 0.28f), 16, 1.05f, 1.45f);
        }

        private static void PlayBurst(string effectName, Vector3 position, Color tint,
            int particleCount, float speed, float size)
        {
            GameSession session = GameSessionBootstrap.Current;
            if (session != null && session.Data != null && session.Data.Settings != null &&
                session.Data.Settings.ReducedMotion) return;

            GameObject effect = new GameObject(effectName);
            effect.transform.position = position;
            ParticleSystem particles = effect.AddComponent<ParticleSystem>();
            // A newly added ParticleSystem may begin its default play-on-awake cycle
            // before its modules are configured. Stop and clear it before changing duration.
            ParticleSystem.MainModule main = particles.main;
            main.playOnAwake = false;
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            main.duration = 0.32f;
            main.loop = false;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.45f, 0.82f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(speed * 0.65f, speed);
            main.startSize = new ParticleSystem.MinMaxCurve(size * 0.07f, size * 0.12f);
            main.startColor = tint;
            main.gravityModifier = 0.16f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 32;
            main.stopAction = ParticleSystemStopAction.Destroy;

            ParticleSystem.EmissionModule emission = particles.emission;
            emission.enabled = true;
            emission.SetBursts(new[]
            {
                new ParticleSystem.Burst(0f, (short)Mathf.Clamp(particleCount, 1, 32))
            });

            ParticleSystem.ShapeModule shape = particles.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.12f;

            ParticleSystem.ColorOverLifetimeModule color = particles.colorOverLifetime;
            color.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(tint, 0f),
                    new GradientColorKey(tint, 1f)
                },
                new[]
                {
                    new GradientAlphaKey(tint.a, 0f),
                    new GradientAlphaKey(tint.a * 0.75f, 0.55f),
                    new GradientAlphaKey(0f, 1f)
                });
            color.color = gradient;

            ParticleSystemRenderer particleRenderer = effect.GetComponent<ParticleSystemRenderer>();
            if (particleRenderer != null)
            {
                particleRenderer.renderMode = ParticleSystemRenderMode.Billboard;
                if (sharedMaterial == null)
                {
                    Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
                    if (shader == null) shader = Shader.Find("Particles/Standard Unlit");
                    if (shader == null) shader = Shader.Find("Sprites/Default");
                    if (shader != null)
                        sharedMaterial = new Material(shader) { name = "MiaMax_Feedback_Unlit" };
                }
                if (sharedMaterial != null) particleRenderer.sharedMaterial = sharedMaterial;
            }

            particles.Play();
        }
    }
}
