using MiaMax.Domain;
using UnityEngine;
namespace MiaMax.Gameplay
{
    public sealed class ProductionStationPrototype : MonoBehaviour
    {
        private ProductionService service;
        private BackyardInteractable interactable;
        private int lastRemaining = -1;
        public void Configure(BackyardInteractable target) { interactable = target; interactable.Interacted += OnInteract; }
        private void Update()
        {
            if (service == null && GameSessionBootstrap.Current != null) service = new ProductionService(GameSessionBootstrap.Current);
            if (service == null) return;
            int remaining = service.RemainingSeconds;
            if (remaining != lastRemaining) { lastRemaining = remaining; UpdateLabel(); }
        }
        private void OnDestroy() { if (interactable != null) interactable.Interacted -= OnInteract; }
        private void OnInteract()
        {
            if (service == null) return;
            var job = service.ActiveJob;
            if (job != null && job.IsActive)
            {
                if (service.RemainingSeconds == 0) service.TryCollect();
            }
            else if (!service.TryStart(ProductionService.CarrotSoup))
                if (!service.TryStart(ProductionService.StrawberryJuice))
                    service.TryStart(ProductionService.WoodenToy);
            UpdateLabel();
        }
        private void UpdateLabel()
        {
            if (interactable == null || service == null) return;
            var job = service.ActiveJob;
            if (job == null || !job.IsActive) interactable.Configure("Workshop", "Start a recipe");
            else if (service.RemainingSeconds > 0) interactable.Configure("Workshop", "Cooking: " + service.RemainingSeconds + "s");
            else interactable.Configure("Workshop", "Collect finished product");
        }
    }
}