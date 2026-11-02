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
            Data.Progress.Experience += amount;
            while (Data.Progress.Experience >= ExperienceForNextLevel(Data.Progress.Level))
            {
                Data.Progress.Experience -= ExperienceForNextLevel(Data.Progress.Level);
                Data.Progress.Level++;
            }
            Notify();
            return true;
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
            return 100 + Math.Max(0, level - 1) * 50;
        }

        private void Notify()
        {
            StateChanged?.Invoke();
        }
    }
}
