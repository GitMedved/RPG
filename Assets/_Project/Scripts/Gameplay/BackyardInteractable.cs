using UnityEngine;
using System;

namespace MiaMax.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public sealed class BackyardInteractable : MonoBehaviour, IBackyardInteractable
    {
        [SerializeField] private string displayName = "Backyard object";
        [SerializeField] private string actionLabel = "Explore";
        public string DisplayName => displayName;
        public string ActionLabel => actionLabel;
        public event Action Interacted;

        public void Configure(string title, string action)
        {
            displayName = title;
            actionLabel = action;
        }

        public void Interact()
        {
            Debug.Log($"Mia interacts with {displayName}: {actionLabel}");
            Interacted?.Invoke();
        }
    }
}
