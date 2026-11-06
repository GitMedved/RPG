using System.Collections.Generic;
using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// M45: lightweight foliage sway and simple transform-based ambient motes.
    /// Uses only core UnityEngine APIs so no optional Particle System module is required.
    /// </summary>
    public sealed class AdventureAnnexAtmospherePrototype : MonoBehaviour
    {
        private readonly List<Transform> animatedParts = new List<Transform>();
        private readonly List<Quaternion> baseRotations = new List<Quaternion>();
        private readonly List<float> phases = new List<float>();
        private readonly List<float> amplitudes = new List<float>();
        private readonly List<Transform> ambientMotes = new List<Transform>();
        private readonly List<Vector3> moteOrigins = new List<Vector3>();
        private readonly List<float> motePhases = new List<float>();
        private readonly List<float> moteScales = new List<float>();
        private Material moteMaterial;
        private bool woodland;
        private bool configured;

        public void Configure(bool isWoodland)
        {
            if (configured) return;
            configured = true;
            woodland = isWoodland;
            CacheFoliage();
            CreateAmbientMotes();
        }

        private void Update()
        {
            if (!configured) return;

            float time = Time.time;
            for (int i = 0; i < animatedParts.Count; i++)
            {
                Transform part = animatedParts[i];
                if (part == null) continue;

                float sway = Mathf.Sin(time * (woodland ? 0.62f : 0.82f) + phases[i]) * amplitudes[i];
                float secondary = Mathf.Sin(time * 0.43f + phases[i] * 1.7f) * amplitudes[i] * 0.35f;
                part.localRotation = baseRotations[i] * Quaternion.Euler(sway, 0f, secondary);
            }

            for (int i = 0; i < ambientMotes.Count; i++)
            {
                Transform mote = ambientMotes[i];
                if (mote == null) continue;
                float phase = time * (woodland ? 0.42f : 0.31f) + motePhases[i];
                Vector3 origin = moteOrigins[i];
                mote.localPosition = origin + new Vector3(
                    Mathf.Sin(phase) * 0.12f,
                    Mathf.Sin(phase * 1.7f) * (woodland ? 0.16f : 0.10f),
                    Mathf.Cos(phase * 0.8f) * 0.10f);
                float pulse = 0.82f + Mathf.Sin(phase * 1.3f) * 0.18f;
                mote.localScale = Vector3.one * moteScales[i] * pulse;
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

        private void CreateAmbientMotes()
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Unlit/Color");
            if (shader == null) return;

            moteMaterial = new Material(shader);
            Color moteColor = woodland
                ? new Color(1f, 0.78f, 0.32f, 1f)
                : new Color(1f, 0.91f, 0.62f, 1f);
            moteMaterial.color = moteColor;
            if (moteMaterial.HasProperty("_BaseColor")) moteMaterial.SetColor("_BaseColor", moteColor);
            if (moteMaterial.HasProperty("_EmissionColor"))
            {
                moteMaterial.EnableKeyword("_EMISSION");
                moteMaterial.SetColor("_EmissionColor", moteColor * (woodland ? 1.15f : 0.55f));
            }

            int count = woodland ? 7 : 5;
            Vector3 center = woodland
                ? new Vector3(-28f, 1.4f, -3.5f)
                : new Vector3(11.5f, 1.0f, 9.5f);
            Vector3 spread = woodland
                ? new Vector3(4.5f, 1.3f, 4f)
                : new Vector3(1.8f, 0.7f, 1.8f);

            for (int i = 0; i < count; i++)
            {
                float angle = i * 2.3999632f;
                Vector3 origin = center + new Vector3(
                    Mathf.Cos(angle) * spread.x,
                    Mathf.Sin(angle * 1.7f) * spread.y,
                    Mathf.Sin(angle) * spread.z);

                GameObject moteObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                moteObject.name = woodland ? "Woodland_Firefly_Mote" : "Garden_Pollen_Mote";
                moteObject.transform.SetParent(transform, false);
                moteObject.transform.localPosition = origin;
                float scale = woodland ? 0.045f : 0.026f;
                moteObject.transform.localScale = Vector3.one * scale;
                Collider moteCollider = moteObject.GetComponent<Collider>();
                if (moteCollider != null) Destroy(moteCollider);
                Renderer moteRenderer = moteObject.GetComponent<Renderer>();
                moteRenderer.sharedMaterial = moteMaterial;
                moteRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                moteRenderer.receiveShadows = false;

                ambientMotes.Add(moteObject.transform);
                moteOrigins.Add(origin);
                motePhases.Add(angle);
                moteScales.Add(scale);
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < ambientMotes.Count; i++)
                if (ambientMotes[i] != null) ambientMotes[i].gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            for (int i = 0; i < ambientMotes.Count; i++)
                if (ambientMotes[i] != null) ambientMotes[i].gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            if (moteMaterial == null) return;
            if (Application.isPlaying) Destroy(moteMaterial);
            else DestroyImmediate(moteMaterial);
        }
    }
}