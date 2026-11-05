using MiaMax.Domain;
using MiaMax.Domain.Collections;
using MiaMax.Domain.Progression;
using MiaMax.Infrastructure.Save;
using MiaMax.Infrastructure.Audio;
using UnityEngine;

namespace MiaMax.Gameplay
{
    public sealed class BackyardCollectiblePrototype : MonoBehaviour
    {
        private string collectibleId;
        private string displayTitle;
        private BackyardInteractable interactable;
        private CollectionService service;
        private bool collected;

        public void Configure(string id, string title, BackyardInteractable target)
        {
            collectibleId = id;
            displayTitle = title;
            interactable = target;
            interactable.Interacted += OnInteract;
        }

        private void Update()
        {
            if (service == null && GameSessionBootstrap.Current != null)
            {
                service = new CollectionService(GameSessionBootstrap.Current);
                collected = service.IsDiscovered(collectibleId);
                if (collected) gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (interactable != null) interactable.Interacted -= OnInteract;
        }

        private void OnInteract()
        {
            if (service == null || collected) return;
            if (!service.Discover(collectibleId)) return;
            new AchievementService(GameSessionBootstrap.Current).GetSummary();
            collected = true;
            GameVoiceDirector.PlayMax("max_meow_curious_01");
            Debug.Log("Keepsake found: " + displayTitle);
            gameObject.SetActive(false);
        }
    }
}