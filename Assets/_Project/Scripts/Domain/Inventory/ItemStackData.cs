using System;

namespace MiaMax.Domain.Inventory
{
    [Serializable]
    public sealed class ItemStackData
    {
        public string ItemId;
        public int Quantity;

        public ItemStackData() { }

        public ItemStackData(string itemId, int quantity)
        {
            ItemId = itemId;
            Quantity = quantity;
        }
    }
}
