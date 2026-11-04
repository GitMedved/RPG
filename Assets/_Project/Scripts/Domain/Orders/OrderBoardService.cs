using System;
using System.Collections.Generic;
using System.Linq;
using MiaMax.Domain.Orders;
using MiaMax.Domain.Progression;
namespace MiaMax.Domain
{
    public sealed class OrderBoardService
    {
        private sealed class OrderDefinition
        {
            public string Id, ItemId;
            public int Quantity, Coins, Experience;
        }
        private static readonly OrderDefinition[] Orders =
        {
            new OrderDefinition { Id = "order_soup_01", ItemId = "product_carrot_soup", Quantity = 1, Coins = 12, Experience = 10 },
            new OrderDefinition { Id = "order_juice_01", ItemId = "product_strawberry_juice", Quantity = 1, Coins = 16, Experience = 12 },
            new OrderDefinition { Id = "order_toy_01", ItemId = "product_wooden_toy", Quantity = 1, Coins = 14, Experience = 11 }
        };
        private readonly GameSession session;
        public OrderBoardService(GameSession session)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            if (session.Data.CompletedOrders == null) session.Data.CompletedOrders = new List<CompletedOrderData>();
        }
        public bool IsCompleted(string id) => session.Data.CompletedOrders.Any(x => x != null && x.OrderId == id);
        public bool CanComplete(string id)
        {
            var order = Orders.FirstOrDefault(x => x.Id == id);
            if (order == null || IsCompleted(id)) return false;
            var stack = session.Data.Inventory.Find(x => x.ItemId == order.ItemId);
            return stack != null && stack.Quantity >= order.Quantity;
        }
        public bool TryComplete(string id)
        {
            var order = Orders.FirstOrDefault(x => x.Id == id);
            if (order == null || IsCompleted(id) || !session.TryRemoveItem(order.ItemId, order.Quantity)) return false;
            session.Data.CompletedOrders.Add(new CompletedOrderData { OrderId = id, CompletedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds() });
            session.TryAddCoins(order.Coins);
            session.AddExperience(order.Experience);
            new TutorialService(session).TryAdvance(TutorialService.Deliver);
            new DailyTaskService(session).RecordDelivery();
            session.NotifyExternalStateChanged();
            return true;
        }
        public string GetStatusSummary()
        {
            int done = Orders.Count(x => IsCompleted(x.Id));
            var ready = Orders
                .Where(x => !IsCompleted(x.Id) && CanComplete(x.Id))
                .Select(x => x.ItemId == "product_carrot_soup" ? "Soup" :
                    x.ItemId == "product_strawberry_juice" ? "Juice" : "Toy")
                .ToArray();
            return "Orders: " + done + "/" + Orders.Length + " delivered. Ready: " +
                (ready.Length > 0 ? string.Join(", ", ready) : "none") + ".";
        }
        public string FirstAvailableOrder()
        {
            foreach (var order in Orders)
                if (!IsCompleted(order.Id) && CanComplete(order.Id)) return order.Id;
            return null;
        }
    }
}