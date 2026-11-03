using UnityEngine;

namespace MiaMax.Gameplay
{
    public sealed class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private float interactionRadius = 2.2f;
        [SerializeField] private float interactionCooldown = 0.25f;
        private BackyardInteractable nearest;
        private float nextInteractionTime;

        private void Update()
        {
            nearest = FindNearest();
            if (nearest == null || Time.time < nextInteractionTime) return;

            bool keyboardPressed = Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space);
            bool touchPressed = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began &&
                                Input.GetTouch(0).position.x > Screen.width * 0.35f;
            if (keyboardPressed || touchPressed) TryInteract();
        }

        public bool HasNearbyTarget => nearest != null;
        public string NearbyPrompt => nearest == null
            ? string.Empty
            : nearest.DisplayName + " · " + nearest.ActionLabel;

        public bool TryInteract()
        {
            if (nearest == null || Time.time < nextInteractionTime) return false;
            nearest.Interact();
            nextInteractionTime = Time.time + interactionCooldown;
            return true;
        }

        private BackyardInteractable FindNearest()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, interactionRadius);
            BackyardInteractable best = null;
            float bestDistance = float.MaxValue;
            foreach (Collider hit in hits)
            {
                BackyardInteractable candidate = hit.GetComponent<BackyardInteractable>();
                if (candidate == null) continue;
                float distance = (candidate.transform.position - transform.position).sqrMagnitude;
                if (distance < bestDistance) { bestDistance = distance; best = candidate; }
            }
            return best;
        }

    }
}
