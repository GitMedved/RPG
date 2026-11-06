using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiaMax.Domain.Progression
{
    public enum BackyardContentKind
    {
        Crop,
        Recipe,
        Order,
        Collectible,
        StoryGoal,
        MiniGame
    }

    [Serializable]
    public sealed class BackyardContentEntry
    {
        public string Id;
        public BackyardContentKind Kind;
        public string DisplayName;
        public string AreaId = "backyard";
        public string UnlockAreaId;
        public string RequiredItemId;
        [Min(0)] public int RequiredQuantity;
        public string RewardItemId;
        [Min(0)] public int RewardQuantity;
        [Min(0)] public int RewardCoins;
        [Min(0)] public int RewardExperience;
    }

    /// <summary>
    /// Authorable first-release content index. It stores references/requirements only;
    /// runtime domain services remain authoritative for inventory, unlocks and rewards.
    /// </summary>
    [CreateAssetMenu(fileName = "BackyardContentCatalog", menuName = "Mia & Max/Backyard Content Catalog")]
    public sealed class BackyardContentCatalog : ScriptableObject
    {
        [SerializeField] private List<BackyardContentEntry> entries = new List<BackyardContentEntry>();

        public IReadOnlyList<BackyardContentEntry> Entries => entries;

        public bool TryGet(string id, out BackyardContentEntry entry)
        {
            entry = null;
            if (string.IsNullOrWhiteSpace(id)) return false;
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i] == null || !string.Equals(entries[i].Id, id, StringComparison.Ordinal))
                    continue;
                entry = entries[i];
                return true;
            }
            return false;
        }

        public List<string> Validate()
        {
            var issues = new List<string>();
            var ids = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < entries.Count; i++)
            {
                BackyardContentEntry entry = entries[i];
                if (entry == null)
                {
                    issues.Add("Entry " + i + " is null.");
                    continue;
                }
                if (string.IsNullOrWhiteSpace(entry.Id))
                    issues.Add("Entry " + i + " has no stable ID.");
                else if (!ids.Add(entry.Id))
                    issues.Add("Duplicate content ID: " + entry.Id);
                if (string.IsNullOrWhiteSpace(entry.DisplayName))
                    issues.Add("Entry " + i + " (" + entry.Id + ") has no display name.");
                if (string.IsNullOrWhiteSpace(entry.AreaId))
                    issues.Add("Entry " + i + " (" + entry.Id + ") has no area ID.");
                if (entry.RequiredQuantity < 0 || entry.RewardQuantity < 0 ||
                    entry.RewardCoins < 0 || entry.RewardExperience < 0)
                    issues.Add("Entry " + i + " (" + entry.Id + ") has a negative quantity or reward.");
                if (entry.RewardQuantity > 0 && string.IsNullOrWhiteSpace(entry.RewardItemId))
                    issues.Add("Entry " + i + " (" + entry.Id + ") has reward quantity but no reward item ID.");
            }
            return issues;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (entries == null) entries = new List<BackyardContentEntry>();
            List<string> issues = Validate();
            for (int i = 0; i < issues.Count; i++)
                Debug.LogWarning("[Mia & Max Content Catalog] " + issues[i], this);
        }
#endif
    }
}
