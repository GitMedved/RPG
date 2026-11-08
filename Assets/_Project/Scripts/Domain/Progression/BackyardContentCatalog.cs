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

        [NonSerialized] private Dictionary<string, BackyardContentEntry> lookup;

        public IReadOnlyList<BackyardContentEntry> Entries => entries ?? (entries = new List<BackyardContentEntry>());
        public int Count => Entries.Count;

        private void OnEnable()
        {
            lookup = null;
        }

        public bool TryGet(string id, out BackyardContentEntry entry)
        {
            entry = null;
            if (string.IsNullOrWhiteSpace(id)) return false;
            EnsureLookup();
            return lookup.TryGetValue(id.Trim(), out entry);
        }

        public bool TryGet(string id, BackyardContentKind kind, out BackyardContentEntry entry)
        {
            if (TryGet(id, out entry) && entry.Kind == kind) return true;
            entry = null;
            return false;
        }

        public IReadOnlyList<BackyardContentEntry> GetByKind(BackyardContentKind kind)
        {
            var matches = new List<BackyardContentEntry>();
            for (int i = 0; i < Entries.Count; i++)
                if (Entries[i] != null && Entries[i].Kind == kind)
                    matches.Add(Entries[i]);
            return matches;
        }

        public bool TryAdd(BackyardContentEntry entry)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.Id) || TryGet(entry.Id, out _))
                return false;
            Entries.Add(entry);
            lookup = null;
            return true;
        }

        private void EnsureLookup()
        {
            if (lookup != null) return;
            lookup = new Dictionary<string, BackyardContentEntry>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < Entries.Count; i++)
            {
                BackyardContentEntry entry = Entries[i];
                if (entry == null || string.IsNullOrWhiteSpace(entry.Id)) continue;
                string id = entry.Id.Trim();
                if (!lookup.ContainsKey(id)) lookup.Add(id, entry);
            }
        }

        public List<string> Validate()
        {
            var issues = new List<string>();
            var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
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
                else if (!string.Equals(entry.Id, entry.Id.Trim(), StringComparison.Ordinal))
                    issues.Add("Entry " + i + " (" + entry.Id + ") has leading or trailing whitespace in its ID.");
                else if (!ids.Add(entry.Id))
                    issues.Add("Duplicate content ID (IDs are case-insensitive): " + entry.Id);
                if (!Enum.IsDefined(typeof(BackyardContentKind), entry.Kind))
                    issues.Add("Entry " + i + " (" + entry.Id + ") has an unknown content kind.");
                if (string.IsNullOrWhiteSpace(entry.DisplayName))
                    issues.Add("Entry " + i + " (" + entry.Id + ") has no display name.");
                if (string.IsNullOrWhiteSpace(entry.AreaId))
                    issues.Add("Entry " + i + " (" + entry.Id + ") has no area ID.");
                if (entry.RequiredQuantity < 0 || entry.RewardQuantity < 0 ||
                    entry.RewardCoins < 0 || entry.RewardExperience < 0)
                    issues.Add("Entry " + i + " (" + entry.Id + ") has a negative quantity or reward.");
                if (entry.RequiredQuantity > 0 && string.IsNullOrWhiteSpace(entry.RequiredItemId))
                    issues.Add("Entry " + i + " (" + entry.Id + ") has a required quantity but no required item ID.");
                if (entry.RewardQuantity > 0 && string.IsNullOrWhiteSpace(entry.RewardItemId))
                    issues.Add("Entry " + i + " (" + entry.Id + ") has reward quantity but no reward item ID.");
                if (!string.IsNullOrWhiteSpace(entry.UnlockAreaId) &&
                    string.Equals(entry.AreaId, entry.UnlockAreaId, StringComparison.Ordinal))
                    issues.Add("Entry " + i + " (" + entry.Id + ") unlocks its own area.");
            }
            return issues;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (entries == null) entries = new List<BackyardContentEntry>();
            lookup = null;
            List<string> issues = Validate();
            for (int i = 0; i < issues.Count; i++)
                Debug.LogWarning("[Mia & Max Content Catalog] " + issues[i], this);
        }
#endif
    }
}
