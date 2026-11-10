using MiaMax.Domain;
using MiaMax.Infrastructure.Save;
using MiaMax.Infrastructure.Audio;
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
            session.Data.CollectedGardenResources.Add(resourceId);
            if (!session.AddItem(itemId, 1))
            {
                session.Data.CollectedGardenResources.Remove(resourceId);
                return;
            }
            session.AddExperience(3);
            session.TryAddCoins(2);
            new MiaMax.Domain.Progression.DailyTaskService(session).RecordExploration();
            session.NotifyExternalStateChanged();
            new MiaMax.Domain.Progression.AchievementService(session).GetSummary();
            collected = true;
            GameAudioService.Instance?.Play(GameAudioCue.Collectible, 0.85f);
            GameVoiceDirector.PlayMax("max_meow_happy_01");
            Debug.Log("Collected " + displayTitle + "! +1 " + itemId + ", +3 XP, +2 coins.");
            gameObject.SetActive(false);
        }
    }
}
