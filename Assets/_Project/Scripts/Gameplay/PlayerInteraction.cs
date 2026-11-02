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
            if (keyboardPressed || touchPressed)
            {
                nearest.Interact();
                nextInteractionTime = Time.time + interactionCooldown;
            }
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

        private void OnGUI()
        {
            if (nearest == null) return;
            GUIStyle style = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Max(14, Screen.height / 48),
                normal = { textColor = Color.white }
            };
            GUI.Box(new Rect(Screen.width * 0.5f - 130f, Screen.height - 100f, 260f, 54f),
                $"{nearest.DisplayName}  ·  {nearest.ActionLabel}  [E]", style);
        }
    }
}
