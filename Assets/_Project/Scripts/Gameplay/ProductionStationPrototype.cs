using MiaMax.Domain;
using MiaMax.Infrastructure.Save;
using MiaMax.Infrastructure.Audio;
using MiaMax.Domain.Progression;
using UnityEngine;
namespace MiaMax.Gameplay
{
    public sealed class ProductionStationPrototype : MonoBehaviour
    {
        private ProductionService service;
        private BackyardInteractable interactable;
        private int lastRemaining = -1;
        private UpgradeService upgrades;
        public void Configure(BackyardInteractable target) { interactable = target; interactable.Interacted += OnInteract; }
        private void Update()
        {
            if (service == null && GameSessionBootstrap.Current != null)
            {
                service = new ProductionService(GameSessionBootstrap.Current);
                upgrades = new UpgradeService(GameSessionBootstrap.Current);
            }
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
                if (service.RemainingSeconds == 0 && service.TryCollect())
                    GameVoiceDirector.PlayMia("mia_success_01");
            }
            else
            {
                bool started = service.TryStart(ProductionService.CarrotSoup);
                if (!started) started = service.TryStart(ProductionService.StrawberryJuice);
                if (!started) started = service.TryStart(ProductionService.WoodenToy);
                if (started) GameVoiceDirector.PlayMia("mia_interact_01");
            }
            UpdateLabel();
        }
        private void UpdateLabel()
        {
            if (interactable == null || service == null) return;
            var job = service.ActiveJob;
            if (job == null || !job.IsActive)
                interactable.Configure("Workshop", "Start a recipe" +
                    (upgrades != null && upgrades.WorkshopLevel > 0 ? " · Lv." + upgrades.WorkshopLevel + " speed bonus" : string.Empty));
            else if (service.RemainingSeconds > 0)
                interactable.Configure("Workshop", "Cooking: " + service.RemainingSeconds + "s · " +
                    Mathf.RoundToInt(service.Progress01 * 100f) + "%");
            else interactable.Configure("Workshop", "Collect finished product");
        }
    }
}