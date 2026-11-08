using System;
using System.Collections.Generic;
using System.Linq;
using MiaMax.Domain.Inventory;

namespace MiaMax.Domain
{
    /// <summary>
    /// Read/query facade for the player's canonical inventory stacks.
    /// Mutations are delegated to GameSession so save listeners see validated changes.
    /// </summary>
    public sealed class InventoryService
    {
        private readonly GameSession session;

        public InventoryService(GameSession session)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            if (session.Data.Inventory == null)
                session.Data.Inventory = new List<ItemStackData>();
        }

        public int Count => session.Data.Inventory.Count;

        public int GetQuantity(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId)) return 0;
            ItemStackData stack = session.Data.Inventory.FirstOrDefault(
                x => x != null && string.Equals(x.ItemId, itemId, StringComparison.Ordinal));
            return stack != null ? Math.Max(0, stack.Quantity) : 0;
        }

        public bool Has(string itemId, int quantity = 1)
        {
            return quantity > 0 && GetQuantity(itemId) >= quantity;
        }

        public bool TryAdd(string itemId, int quantity)
        {
            return session.AddItem(itemId, quantity);
        }

        public bool TryConsume(string itemId, int quantity)
        {
            return session.TryRemoveItem(itemId, quantity);
        }

        /// <summary>Returns detached copies; callers cannot mutate the saved inventory.</summary>
        public IReadOnlyList<ItemStackData> GetSnapshot()
        {
            var snapshot = new List<ItemStackData>(session.Data.Inventory.Count);
            for (int i = 0; i < session.Data.Inventory.Count; i++)
            {
                ItemStackData stack = session.Data.Inventory[i];
                if (stack != null && !string.IsNullOrWhiteSpace(stack.ItemId) && stack.Quantity > 0)
                    snapshot.Add(new ItemStackData(stack.ItemId, stack.Quantity));
            }
            return snapshot;
        }
    }
}
