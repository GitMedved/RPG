using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiaMax.Gameplay
{
    /// <summary>
    /// Lightweight modal navigation history for Canvas screens.
    /// A screen is identified by a stable route key; presentation remains owned by the HUD.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MobileUiNavigationStack : MonoBehaviour
    {
        [SerializeField] private string homeRoute = "home";
        private readonly List<string> history = new List<string>();

        public string CurrentRoute => history.Count == 0 ? homeRoute : history[history.Count - 1];
        public int Depth => history.Count;
        public event Action<string> RouteChanged;

        private void Awake()
        {
            if (history.Count == 0 && !string.IsNullOrWhiteSpace(homeRoute))
                history.Add(homeRoute);
        }

        public bool Push(string route)
        {
            if (string.IsNullOrWhiteSpace(route)) return false;
            string normalized = route.Trim();
            if (string.Equals(CurrentRoute, normalized, StringComparison.Ordinal)) return false;
            history.Add(normalized);
            RouteChanged?.Invoke(CurrentRoute);
            return true;
        }

        public bool Back()
        {
            if (history.Count <= 1) return false;
            history.RemoveAt(history.Count - 1);
            RouteChanged?.Invoke(CurrentRoute);
            return true;
        }

        public void ResetToHome()
        {
            history.Clear();
            if (!string.IsNullOrWhiteSpace(homeRoute)) history.Add(homeRoute);
            RouteChanged?.Invoke(CurrentRoute);
        }

        public bool IsCurrent(string route)
        {
            return !string.IsNullOrWhiteSpace(route) &&
                   string.Equals(CurrentRoute, route, StringComparison.Ordinal);
        }
    }
}
