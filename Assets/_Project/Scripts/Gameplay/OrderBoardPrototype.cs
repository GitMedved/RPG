using MiaMax.Domain;
using MiaMax.Infrastructure.Save;
using UnityEngine;
namespace MiaMax.Gameplay
{
    public sealed class OrderBoardPrototype : MonoBehaviour
    {
        private OrderBoardService service;
        private BackyardInteractable interactable;
        public void Configure(BackyardInteractable target) { interactable = target; interactable.Interacted += OnInteract; }
        private void Update()
        {
            if (service == null && GameSessionBootstrap.Current != null) { service = new OrderBoardService(GameSessionBootstrap.Current); UpdateLabel(); }
        }
        private void OnDestroy() { if (interactable != null) interactable.Interacted -= OnInteract; }
        private void OnInteract()
        {
            if (service == null) return;
            string available = service.FirstAvailableOrder();
            if (available != null) service.TryComplete(available);
            UpdateLabel();
        }
        private void UpdateLabel()
        {
            if (interactable != null && service != null) interactable.Configure("Order board", service.GetStatusSummary() + " Interact to deliver a ready order.");
        }
    }
}