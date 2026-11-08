using MiaMax.Domain;
using MiaMax.Domain.Farming;
using MiaMax.Domain.Progression;
using MiaMax.Infrastructure.Save;
using MiaMax.Infrastructure.Audio;
using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// Reference-friendly low-poly crop visuals for the temporary backyard garden.
    /// The crop is rebuilt only when its saved item ID changes; growth stages scale the plant.
    /// Replace this procedural presentation with authored crop prefabs when production art arrives.
    /// </summary>
    public sealed class FarmPlotPrototype : MonoBehaviour
    {
        [SerializeField] private string plotId;
        private FarmingService farming;
        private BackyardInteractable interactable;
        private Transform cropRoot;
        private string lastCropId;
        private int lastStage = -1;
        private float nextRefreshTime;
        private Material leafMaterial;
        private Material leafLightMaterial;
        private Material carrotMaterial;
        private Material tomatoMaterial;
        private Material berryMaterial;
        private Material flowerMaterial;

        public void Configure(string id, BackyardInteractable target)
        {
            plotId = id;
            interactable = target;

            leafMaterial = Make(new Color(0.18f, 0.43f, 0.16f));
            leafLightMaterial = Make(new Color(0.34f, 0.62f, 0.22f));
            carrotMaterial = Make(new Color(1.0f, 0.39f, 0.08f));
            tomatoMaterial = Make(new Color(0.86f, 0.12f, 0.10f));
            berryMaterial = Make(new Color(0.91f, 0.18f, 0.29f));
            flowerMaterial = Make(new Color(1.0f, 0.78f, 0.30f));

            GameObject visual = new GameObject("Crop_Visual");
            visual.transform.SetParent(transform, false);
            visual.transform.localPosition = new Vector3(0f, 0.28f, 0f);
            cropRoot = visual.transform;
            cropRoot.gameObject.SetActive(false);

            if (interactable != null) interactable.Interacted += OnInteract;
            Refresh();
        }

        private void Start()
        {
            if (GameSessionBootstrap.Current != null)
                farming = new FarmingService(GameSessionBootstrap.Current);
            Refresh();
        }

        private void Update()
        {
            if (farming == null && GameSessionBootstrap.Current != null)
                farming = new FarmingService(GameSessionBootstrap.Current);
            if (Time.unscaledTime >= nextRefreshTime)
            {
                nextRefreshTime = Time.unscaledTime + 0.25f;
                Refresh();
            }
        }

        private void OnDestroy()
        {
            if (interactable != null) interactable.Interacted -= OnInteract;
        }

        private void OnInteract()
        {
            if (farming == null || GameSessionBootstrap.Current == null) return;

            FarmPlotData plot = farming.GetPlot(plotId);
            if (!plot.IsPlanted)
            {
                string[] crops = { FarmingService.Carrot, FarmingService.Tomato, FarmingService.Strawberry };
                foreach (string crop in crops)
                {
                    if (!farming.TryPlant(plotId, crop)) continue;
                    new TutorialService(GameSessionBootstrap.Current).TryAdvance(TutorialService.Plant);
                    GameVoiceDirector.PlayMia("mia_plant_01");
                    break;
                }
            }
            else if (farming.TryHarvest(plotId, out _))
            {
                new TutorialService(GameSessionBootstrap.Current).TryAdvance(TutorialService.Harvest);
                GameVoiceDirector.PlayMia("mia_harvest_01");
            }

            Refresh();
        }

        private void Refresh()
        {
            if (farming == null || cropRoot == null) return;

            FarmPlotData plot = farming.GetPlot(plotId);
            int stage = farming.GrowthStage(plot);
            float progress = farming.GrowthProgress01(plot);
            string cropId = plot.IsPlanted ? plot.CropId : null;

            if (cropId != lastCropId)
            {
                RebuildCrop(cropId);
                lastCropId = cropId;
                lastStage = -1;
            }

            if (stage != lastStage)
            {
                cropRoot.gameObject.SetActive(stage > 0);
                lastStage = stage;
            }

            if (plot.IsPlanted)
            {
                float growth = stage <= 1 ? Mathf.Lerp(0.42f, 0.60f, progress * 2f) :
                    stage == 2 ? Mathf.Lerp(0.60f, 0.82f, (progress - 0.5f) * 2f) : 1f;
                cropRoot.localScale = Vector3.one * growth;
            }
            else cropRoot.localScale = Vector3.zero;

            if (interactable != null)
            {
                string action = !plot.IsPlanted ? "Plant a seed" :
                    stage >= 3 ? "Harvest crop" :
                    "Growing · " + farming.SecondsUntilHarvest(plot) + "s";
                interactable.Configure("Garden plot", action);
            }
        }

        private void RebuildCrop(string cropId)
        {
            for (int i = cropRoot.childCount - 1; i >= 0; i--)
                Destroy(cropRoot.GetChild(i).gameObject);

            if (string.IsNullOrEmpty(cropId)) return;

            if (cropId == FarmingService.Carrot)
                BuildCarrot();
            else if (cropId == FarmingService.Tomato)
                BuildTomato();
            else if (cropId == FarmingService.Strawberry)
                BuildStrawberry();
        }

        private void BuildCarrot()
        {
            // Orange root sits partly inside the soil, with a leafy crown above it.
            Part("Carrot_Root", PrimitiveType.Sphere, new Vector3(0f, -0.015f, 0f),
                new Vector3(0.24f, 0.39f, 0.22f), carrotMaterial);
            Part("Carrot_Crown", PrimitiveType.Sphere, new Vector3(0f, 0.18f, 0f),
                new Vector3(0.16f, 0.11f, 0.16f), leafMaterial);

            for (int i = 0; i < 5; i++)
            {
                float angle = i * 72f;
                float radians = angle * Mathf.Deg2Rad;
                Vector3 direction = new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians));
                Transform leaf = Part("Carrot_Leaf", PrimitiveType.Capsule,
                    new Vector3(direction.x * 0.13f, 0.43f + (i % 2) * 0.04f, direction.z * 0.13f),
                    new Vector3(0.075f, 0.27f, 0.075f), i % 2 == 0 ? leafMaterial : leafLightMaterial);
                leaf.localRotation = Quaternion.Euler(direction.z * 35f, 0f, -direction.x * 35f);
            }
        }

        private void BuildTomato()
        {
            Part("Tomato_Main_Stem", PrimitiveType.Cylinder, new Vector3(0f, 0.34f, 0f),
                new Vector3(0.055f, 0.35f, 0.055f), leafMaterial);

            for (int i = 0; i < 4; i++)
            {
                float angle = i * 90f + 20f;
                float radians = angle * Mathf.Deg2Rad;
                Vector3 direction = new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians));
                Transform leaf = Part("Tomato_Leaf", PrimitiveType.Sphere,
                    new Vector3(direction.x * 0.22f, 0.45f + (i % 2) * 0.12f, direction.z * 0.22f),
                    new Vector3(0.28f, 0.07f, 0.16f), i % 2 == 0 ? leafMaterial : leafLightMaterial);
                leaf.localRotation = Quaternion.Euler(0f, -angle, 0f);
                Part("Tomato_Fruit", PrimitiveType.Sphere,
                    new Vector3(direction.x * 0.16f, 0.22f + (i % 2) * 0.10f, direction.z * 0.16f),
                    Vector3.one * 0.20f, tomatoMaterial);
            }

            Part("Tomato_Top_Flower", PrimitiveType.Sphere, new Vector3(0f, 0.69f, 0f),
                new Vector3(0.12f, 0.08f, 0.12f), flowerMaterial);
        }

        private void BuildStrawberry()
        {
            Part("Strawberry_Crown", PrimitiveType.Sphere, new Vector3(0f, 0.18f, 0f),
                new Vector3(0.13f, 0.09f, 0.13f), leafMaterial);

            for (int i = 0; i < 5; i++)
            {
                float angle = i * 72f;
                float radians = angle * Mathf.Deg2Rad;
                Vector3 direction = new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians));
                Transform leaf = Part("Strawberry_Leaf", PrimitiveType.Sphere,
                    new Vector3(direction.x * 0.18f, 0.28f, direction.z * 0.18f),
                    new Vector3(0.22f, 0.055f, 0.12f), i % 2 == 0 ? leafMaterial : leafLightMaterial);
                leaf.localRotation = Quaternion.Euler(0f, -angle, 0f);
            }

            Part("Strawberry_Fruit_Left", PrimitiveType.Sphere, new Vector3(-0.13f, 0.10f, 0.08f),
                new Vector3(0.17f, 0.20f, 0.16f), berryMaterial);
            Part("Strawberry_Fruit_Right", PrimitiveType.Sphere, new Vector3(0.13f, 0.10f, -0.08f),
                new Vector3(0.17f, 0.20f, 0.16f), berryMaterial);
            Part("Strawberry_Blossom", PrimitiveType.Sphere, new Vector3(0f, 0.40f, 0f),
                new Vector3(0.10f, 0.08f, 0.10f), flowerMaterial);
        }

        private Transform Part(string partName, PrimitiveType type, Vector3 localPosition,
            Vector3 localScale, Material material)
        {
            GameObject part = GameObject.CreatePrimitive(type);
            part.name = partName;
            part.transform.SetParent(cropRoot, false);
            part.transform.localPosition = localPosition;
            part.transform.localScale = localScale;
            part.GetComponent<Renderer>().sharedMaterial = material;
            Collider collider = part.GetComponent<Collider>();
            if (collider != null) Destroy(collider);
            return part.transform;
        }

        private static Material Make(Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            return new Material(shader) { color = color };
        }
    }
}
