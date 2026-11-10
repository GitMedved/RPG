using MiaMax.Domain;
using MiaMax.Domain.Progression;
using MiaMax.Infrastructure.Save;
using MiaMax.Infrastructure.Audio;
using UnityEngine;

namespace MiaMax.Gameplay
{
    public sealed class QuestBoardPrototype : MonoBehaviour
    {
        private QuestService service;
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
                service = new QuestService(GameSessionBootstrap.Current);
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
            string title;
            if (service.TryClaimNext(out title))
            {
                GameAudioService.Instance?.Play(GameAudioCue.Reward, 0.9f);
                GameVoiceDirector.PlayMia("mia_quest_01");
                Debug.Log("Quest reward claimed: " + title);
            }
            else
                Debug.Log("No quest reward is ready yet. " + service.GetSummary());
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            if (interactable != null && service != null)
                interactable.Configure("Adventure journal", service.GetSummary());
        }
    }
}
