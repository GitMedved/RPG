using MiaMax.Domain;
using MiaMax.Domain.Collections;
using MiaMax.Infrastructure.Save;
using MiaMax.Infrastructure.Audio;
using UnityEngine;

namespace MiaMax.Gameplay
{
    public sealed class CollectionBookPrototype : MonoBehaviour
    {
        private CollectionService service;
        private BackyardInteractable interactable;

        public void Configure(BackyardInteractable target)
        {
            interactable = target;
            interactable.Interacted += OnInteract;
        }

        private void Update()
        {
            if (service == null && GameSessionBootstrap.Current != null)
            {
                service = new CollectionService(GameSessionBootstrap.Current);
                UpdateLabel();
            }
        }

        private void OnDestroy()
        {
            if (interactable != null) interactable.Interacted -= OnInteract;
        }

        private void OnInteract()
        {
            if (service == null) return;
            if (service.TryClaimCompletionReward())
            {
                GameAudioService.Instance?.Play(GameAudioCue.Reward, 0.9f);
                GameVoiceDirector.PlayMia("mia_success_01");
                Debug.Log("Collection completed! +25 coins, +30 XP.");
            }
            else
                Debug.Log(service.GetSummary());
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            if (interactable != null && service != null)
                interactable.Configure("Backyard collection", service.GetSummary());
        }
    }
}