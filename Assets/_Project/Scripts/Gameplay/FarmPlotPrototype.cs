using MiaMax.Domain;
using MiaMax.Domain.Farming;
using MiaMax.Domain.Progression;
using MiaMax.Infrastructure.Save;
using UnityEngine;
namespace MiaMax.Gameplay
{
    public sealed class FarmPlotPrototype : MonoBehaviour
    {
        [SerializeField] private string plotId;
        private FarmingService farming;
        private BackyardInteractable interactable;
        private Renderer cropVisual;
        private Material emptyMaterial, growingMaterial, readyMaterial;
        private int lastStage = -1;
        public void Configure(string id, BackyardInteractable target)
        {
            plotId = id; interactable = target;
            emptyMaterial = Make(new Color(.28f,.16f,.08f));
            growingMaterial = Make(new Color(.22f,.62f,.19f));
            readyMaterial = Make(new Color(.95f,.65f,.12f));
            GameObject sprout = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sprout.name = "Crop_Visual_Placeholder";
            sprout.transform.SetParent(transform, false);
            sprout.transform.localPosition = new Vector3(0,.42f,0);
            sprout.transform.localScale = new Vector3(.35f,.5f,.35f);
            Object.Destroy(sprout.GetComponent<Collider>());
            cropVisual = sprout.GetComponent<Renderer>();
            cropVisual.enabled = false;
            interactable.Interacted += OnInteract;
            Refresh();
        }
        private void Start()
        {
            if (GameSessionBootstrap.Current != null) farming = new FarmingService(GameSessionBootstrap.Current);
            Refresh();
        }
        private void Update()
        {
            if (farming == null && GameSessionBootstrap.Current != null) farming = new FarmingService(GameSessionBootstrap.Current);
            Refresh();
        }
        private void OnDestroy()
        {
            if (interactable != null) interactable.Interacted -= OnInteract;
        }
        private void OnInteract()
        {
            if (farming == null) return;
            var plot = farming.GetPlot(plotId);
            if (!plot.IsPlanted)
            {
                string[] crops = { FarmingService.Carrot, FarmingService.Tomato, FarmingService.Strawberry };
                foreach (var crop in crops)
                    if (farming.TryPlant(plotId, crop))
                    {
                        new TutorialService(GameSessionBootstrap.Current).TryAdvance(TutorialService.Plant);
                        break;
                    }
            }
            else if (farming.TryHarvest(plotId, out _))
                new TutorialService(GameSessionBootstrap.Current).TryAdvance(TutorialService.Harvest);
            Refresh();
        }
        private void Refresh()
        {
            if (farming == null || cropVisual == null) return;
            var plot = farming.GetPlot(plotId);
            int stage = farming.GrowthStage(plot);
            cropVisual.enabled = stage > 0;
            if (stage != lastStage)
            {
                cropVisual.transform.localScale = stage == 0 ? Vector3.zero : new Vector3(.18f + stage*.09f,.18f + stage*.12f,.18f + stage*.09f);
                cropVisual.sharedMaterial = stage >= 3 ? readyMaterial : growingMaterial;
                lastStage = stage;
                if (interactable != null)
                    interactable.Configure("Garden plot", !plot.IsPlanted ? "Plant a seed" : stage >= 3 ? "Harvest crop" : "Crop growing");
            }
        }
        private static Material Make(Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            return new Material(shader) { color = color };
        }
    }
}