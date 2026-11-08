using System;
using System.Collections.Generic;
using MiaMax.Domain.Inventory;
using MiaMax.Infrastructure.Save;

namespace MiaMax.Domain
{
    /// <summary>
    /// In-memory player state and validated domain operations.
    /// Presentation code calls these methods instead of editing save fields directly.
    /// </summary>
    public sealed class GameSession
    {
        public PlayerSaveData Data { get; }
        public event Action StateChanged;

        public GameSession(PlayerSaveData data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public bool TryAddCoins(int amount)
        {
            if (amount <= 0 || Data.Progress.Coins > int.MaxValue - amount) return false;
            Data.Progress.Coins += amount;
            Notify();
            return true;
        }

        public bool TrySpendCoins(int amount)
        {
            if (amount <= 0 || Data.Progress.Coins < amount) return false;
            Data.Progress.Coins -= amount;
            Notify();
            return true;
        }

        public bool AddExperience(int amount)
        {
            if (amount <= 0 || Data.Progress.Experience > int.MaxValue - amount) return false;
            ApplyExperience(amount);
            Notify();
            return true;
        }

        /// <summary>Applies a coin/XP reward as one validated state transition.</summary>
        public bool TryGrantRewards(int coins, int experience)
        {
            if (coins < 0 || experience < 0 || (coins == 0 && experience == 0)) return false;
            if (Data.Progress.Coins > int.MaxValue - coins ||
                Data.Progress.Experience > int.MaxValue - experience) return false;
            Data.Progress.Coins += coins;
            if (experience > 0) ApplyExperience(experience);
            Notify();
            return true;
        }

        private void ApplyExperience(int amount)
        {
            long total = (long)Data.Progress.Experience + amount;
            int level = Math.Max(1, Data.Progress.Level);
            const int maxLevel = 100;
            while (level < maxLevel && total >= ExperienceForNextLevel(level))
            {
                total -= ExperienceForNextLevel(level);
                level++;
            }
            Data.Progress.Level = level;
            Data.Progress.Experience = level >= maxLevel
                ? Math.Min((int)total, ExperienceForNextLevel(maxLevel) - 1)
                : (int)Math.Min(int.MaxValue, total);
        }

        public bool AddItem(string itemId, int quantity)
        {
            if (string.IsNullOrWhiteSpace(itemId) || quantity <= 0) return false;
            ItemStackData stack = Data.Inventory.Find(x => x.ItemId == itemId);
            if (stack == null) Data.Inventory.Add(new ItemStackData(itemId, quantity));
            else
            {
                if (stack.Quantity > int.MaxValue - quantity) return false;
                stack.Quantity += quantity;
            }
            Notify();
            return true;
        }

        public bool TryRemoveItem(string itemId, int quantity)
        {
            if (string.IsNullOrWhiteSpace(itemId) || quantity <= 0) return false;
            ItemStackData stack = Data.Inventory.Find(x => x.ItemId == itemId);
            if (stack == null || stack.Quantity < quantity) return false;
            stack.Quantity -= quantity;
            if (stack.Quantity == 0) Data.Inventory.Remove(stack);
            Notify();
            return true;
        }

        public static int ExperienceForNextLevel(int level)
        {
            int safeLevel = Math.Max(1, Math.Min(100, level));
            return 100 + (safeLevel - 1) * 50;
        }

        public void NotifyExternalStateChanged()
        {
            Notify();
        }

        private void Notify()
        {
            StateChanged?.Invoke();
        }
    }
}
