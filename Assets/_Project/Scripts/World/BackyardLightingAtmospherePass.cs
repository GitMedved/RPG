using UnityEngine;
using UnityEngine.Rendering;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// M41: establishes a consistent warm daylight look for the runtime backyard.
    /// Uses the existing prototype camera and sun; does not create extra lights.
    /// </summary>
    public static class BackyardLightingAtmospherePass
    {
        public static void Apply(Camera camera, Light sun)
        {
            ApplyCamera(camera);
            ApplySun(sun);
            ApplyAmbientLighting();
        }

        private static void ApplyCamera(Camera camera)
        {
            if (camera == null) return;

            // A clean, soft sky tone keeps the blockout readable on mobile and avoids
            // relying on whichever default skybox happens to be installed in the project.
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.68f, 0.82f, 0.88f, 1f);
            camera.allowHDR = false;
            camera.allowMSAA = true;
        }

        private static void ApplySun(Light sun)
        {
            if (sun == null) return;

            sun.type = LightType.Directional;
            sun.color = new Color(1f, 0.91f, 0.78f, 1f);
            sun.intensity = 1.05f;
            sun.transform.rotation = Quaternion.Euler(48f, -32f, 0f);
            sun.shadows = LightShadows.Soft;
            sun.shadowStrength = 0.78f;
            sun.shadowBias = 0.05f;
            sun.shadowNormalBias = 0.35f;
            sun.renderMode = LightRenderMode.Auto;
        }

        private static void ApplyAmbientLighting()
        {
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.68f, 0.79f, 0.88f, 1f);
            RenderSettings.ambientEquatorColor = new Color(0.76f, 0.75f, 0.64f, 1f);
            RenderSettings.ambientGroundColor = new Color(0.39f, 0.43f, 0.31f, 1f);
            RenderSettings.ambientIntensity = 1.0f;
            RenderSettings.reflectionIntensity = 0.55f;
            RenderSettings.fog = false;
        }
    }
}
