using MiaMax.Domain;
using MiaMax.Domain.Progression;
using MiaMax.Infrastructure.Save;
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

        public void Configure(Transform playerTransform, Vector3 targetPosition, bool exitPortal, BackyardInteractable targetInteractable)
        {
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
            if (!quests.IsAreaUnlocked("flower_garden"))
            {
                Debug.Log("The Flower Garden unlocks after completing the story quest 'A Busy Little Backyard'.");
                return;
            }

            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;
            player.position = destination;
            if (controller != null) controller.enabled = true;
            Debug.Log(isExit ? "Back in the backyard!" : "Welcome to the Flower Garden!");
        }
    }
}
