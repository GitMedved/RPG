using System.Collections.Generic;
using MiaMax.Domain;
using MiaMax.Infrastructure.Save;
using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>One-time collectible resource in a story-unlocked adventure annex.</summary>
    public sealed class AdventureAreaResourcePrototype : MonoBehaviour
    {
        private string resourceId;
        private string itemId;
        private string displayTitle;
        private BackyardInteractable interactable;
        private GameSession session;
        private bool collected;

        public void Configure(string id, string item, string title, BackyardInteractable target)
        {
            resourceId = id;
            itemId = item;
            displayTitle = title;
            interactable = target;
            if (interactable != null) interactable.Interacted += OnInteract;
        }

        private void Update()
        {
            if (session != null || GameSessionBootstrap.Current == null) return;
            session = GameSessionBootstrap.Current;
            if (session.Data.CollectedTrailResources == null)
                session.Data.CollectedTrailResources = new List<string>();
            collected = session.Data.CollectedTrailResources.Contains(resourceId);
            if (collected) gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (interactable != null) interactable.Interacted -= OnInteract;
        }

        private void OnInteract()
        {
            if (session == null || collected) return;
            if (session.Data.CollectedTrailResources.Contains(resourceId)) return;

            session.Data.CollectedTrailResources.Add(resourceId);
            if (!session.AddItem(itemId, 1))
            {
                session.Data.CollectedTrailResources.Remove(resourceId);
                return;
            }

            session.AddExperience(5);
            session.TryAddCoins(3);
            session.NotifyExternalStateChanged();
            collected = true;
            Debug.Log("Trail discovery: " + displayTitle + ". +1 " + itemId + ", +5 XP, +3 coins.");
            gameObject.SetActive(false);
        }
    }
}
