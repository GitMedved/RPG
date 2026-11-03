using MiaMax.Domain;
using MiaMax.Infrastructure.Save;
using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>One-time resource node in the Flower Garden, persisted in the player save.</summary>
    public sealed class FlowerGardenResourcePrototype : MonoBehaviour
    {
        private string resourceId;
        private string itemId;
        private string displayTitle;
        private BackyardInteractable interactable;
        private bool collected;
        private GameSession session;

        public void Configure(string id, string item, string title, BackyardInteractable target)
        {
            resourceId = id;
            itemId = item;
            displayTitle = title;
            interactable = target;
            interactable.Interacted += OnInteract;
        }

        private void Update()
        {
            if (session != null || GameSessionBootstrap.Current == null) return;
            session = GameSessionBootstrap.Current;
            collected = session.Data.CollectedGardenResources.Contains(resourceId);
            if (collected) gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (interactable != null) interactable.Interacted -= OnInteract;
        }

        private void OnInteract()
        {
            if (session == null || collected || session.Data.CollectedGardenResources.Contains(resourceId)) return;
            if (!session.AddItem(itemId, 1)) return;
            session.Data.CollectedGardenResources.Add(resourceId);
            session.AddExperience(3);
            session.TryAddCoins(2);
            session.NotifyExternalStateChanged();
            collected = true;
            Debug.Log("Collected " + displayTitle + "! +1 " + itemId + ", +3 XP, +2 coins.");
            gameObject.SetActive(false);
        }
    }
}
