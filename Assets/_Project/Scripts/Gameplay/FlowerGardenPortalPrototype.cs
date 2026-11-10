using MiaMax.Domain;
using MiaMax.Domain.Progression;
using MiaMax.Infrastructure.Save;
using MiaMax.Infrastructure.Audio;
using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>Simple gated portal between the backyard and its flower garden annex.</summary>
    public sealed class FlowerGardenPortalPrototype : MonoBehaviour
    {
        private Transform player;
        private Vector3 destination;
        private bool isExit;
        private QuestService quests;
        private BackyardInteractable interactable;
        private string requiredAreaId = "flower_garden";
        private string areaDisplayName = "Flower Garden";

        public void Configure(Transform playerTransform, Vector3 targetPosition, bool exitPortal, BackyardInteractable targetInteractable,
            string unlockAreaId = "flower_garden", string displayName = "Flower Garden")
        {
            requiredAreaId = unlockAreaId;
            areaDisplayName = displayName;
            interactable = targetInteractable;
            if (interactable != null) interactable.Interacted += OnInteract;
            player = playerTransform;
            destination = targetPosition;
            isExit = exitPortal;
        }

        private void Update()
        {
            if (quests == null && GameSessionBootstrap.Current != null)
                quests = new QuestService(GameSessionBootstrap.Current);
        }

        private void OnDestroy()
        {
            if (interactable != null) interactable.Interacted -= OnInteract;
        }

        private void OnInteract()
        {
            if (player == null || quests == null) return;
            if (!quests.IsAreaUnlocked(requiredAreaId))
            {
                Debug.Log(areaDisplayName + " is locked. Complete the preceding story quest to unlock it.");
                return;
            }

            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;
            player.position = destination;
            if (controller != null) controller.enabled = true;
            if (GameAudioService.Instance != null)
            {
                if (isExit)
                {
                    GameAudioService.Instance.SetMusicById("backyard_theme", 0.22f);
                    GameAudioService.Instance.SetAdventureAmbience(null, null);
                }
                else if (requiredAreaId == "woodland_trail")
                {
                    GameAudioService.Instance.SetMusicById("woodland_theme", 0.20f);
                    GameAudioService.Instance.SetAdventureAmbience("woodland_wind", "woodland_birds", 0.12f, 0.10f);
                }
                else
                {
                    GameAudioService.Instance.SetMusicById("flower_garden_theme", 0.20f);
                    GameAudioService.Instance.SetAdventureAmbience(null, null);
                }
                BackyardAmbientPrototype.SetBackyardAudioEnabled(isExit || requiredAreaId != "woodland_trail");
            }
            Debug.Log(isExit ? "Back in the backyard!" : "Welcome to " + areaDisplayName + "!");
        }
    }
}
