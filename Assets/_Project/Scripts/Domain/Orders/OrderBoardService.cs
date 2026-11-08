using System;
using System.Collections.Generic;
using System.Linq;
using MiaMax.Domain.Orders;
using MiaMax.Domain.Progression;
using UnityEngine;

namespace MiaMax.Domain
{
    public sealed class OrderBoardService
    {
        private sealed class OrderDefinition
        {
            public string Id;
            public string ItemId;
            public string DisplayName;
            public string AreaId;
            public int Quantity;
            public int Coins;
            public int Experience;
        }

        private static readonly OrderDefinition[] DefaultOrders =
        {
            new OrderDefinition { Id = "order_soup_01", ItemId = "product_carrot_soup", Quantity = 1, Coins = 12, Experience = 10, DisplayName = "Soup Delivery", AreaId = "backyard" },
            new OrderDefinition { Id = "order_juice_01", ItemId = "product_strawberry_juice", Quantity = 1, Coins = 16, Experience = 12, DisplayName = "Juice Delivery", AreaId = "backyard" },
            new OrderDefinition { Id = "order_toy_01", ItemId = "product_wooden_toy", Quantity = 1, Coins = 14, Experience = 11, DisplayName = "Toy Delivery", AreaId = "backyard" }
        };

        private readonly GameSession session;
        private readonly InventoryService inventory;
        private readonly BackyardContentCatalog catalog;

        public OrderBoardService(GameSession session) : this(session,
            Resources.Load<BackyardContentCatalog>("MiaMax/Content/BackyardContentCatalog")) { }

        public OrderBoardService(GameSession session, BackyardContentCatalog catalog)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            inventory = new InventoryService(session);
            this.catalog = catalog;
            if (session.Data.CompletedOrders == null) session.Data.CompletedOrders = new List<CompletedOrderData>();
        }

        private List<OrderDefinition> GetOrders()
        {
            var result = new List<OrderDefinition>();
            var includedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < DefaultOrders.Length; i++)
            {
                OrderDefinition fallback = DefaultOrders[i];
                if (catalog != null && catalog.TryGet(fallback.Id, BackyardContentKind.Order, out BackyardContentEntry entry))
                {
                    result.Add(new OrderDefinition
                    {
                        Id = entry.Id,
                        ItemId = string.IsNullOrWhiteSpace(entry.RequiredItemId) ? fallback.ItemId : entry.RequiredItemId,
                        Quantity = entry.RequiredQuantity > 0 ? entry.RequiredQuantity : fallback.Quantity,
                        Coins = entry.RewardCoins,
                        Experience = entry.RewardExperience,
                        DisplayName = string.IsNullOrWhiteSpace(entry.DisplayName) ? fallback.DisplayName : entry.DisplayName,
                        AreaId = entry.AreaId
                    });
                }
                else if (IsAreaUnlocked(fallback.AreaId)) result.Add(fallback);
                if (catalog != null && catalog.TryGet(fallback.Id, BackyardContentKind.Order, out BackyardContentEntry areaEntry) && !IsAreaUnlocked(areaEntry.AreaId)) result.RemoveAll(x => x.Id == fallback.Id);
                includedIds.Add(fallback.Id);
            }

            if (catalog != null)
            {
                IReadOnlyList<BackyardContentEntry> authoredOrders = catalog.GetByKind(BackyardContentKind.Order);
                for (int i = 0; i < authoredOrders.Count; i++)
                {
                    BackyardContentEntry entry = authoredOrders[i];
                    if (entry == null || string.IsNullOrWhiteSpace(entry.Id) || includedIds.Contains(entry.Id) ||
                        string.IsNullOrWhiteSpace(entry.RequiredItemId) || entry.RequiredQuantity <= 0 ||
                        (entry.RewardCoins <= 0 && entry.RewardExperience <= 0) || !IsAreaUnlocked(entry.AreaId)) continue;
                    result.Add(new OrderDefinition
                    {
                        Id = entry.Id,
                        ItemId = entry.RequiredItemId,
                        Quantity = entry.RequiredQuantity,
                        Coins = entry.RewardCoins,
                        Experience = entry.RewardExperience,
                        DisplayName = string.IsNullOrWhiteSpace(entry.DisplayName) ? entry.Id : entry.DisplayName,
                        AreaId = entry.AreaId
                    });
                    includedIds.Add(entry.Id);
                }
            }
            return result;
        }

        private bool IsAreaUnlocked(string areaId)
        {
            return string.IsNullOrWhiteSpace(areaId) || new QuestService(session, catalog).IsAreaUnlocked(areaId);
        }

        public bool IsCompleted(string id) => session.Data.CompletedOrders.Any(x => x != null && x.OrderId == id);

        public bool CanComplete(string id)
        {
            OrderDefinition order = GetOrders().FirstOrDefault(x => x.Id == id);
            if (order == null || IsCompleted(id)) return false;
            return inventory.Has(order.ItemId, order.Quantity);
        }

        public bool TryComplete(string id)
        {
            OrderDefinition order = GetOrders().FirstOrDefault(x => x.Id == id);
            if (order == null || IsCompleted(id) ||
                !session.CanConsumeItemAndGrantRewards(order.ItemId, order.Quantity, order.Coins, order.Experience))
                return false;

            session.Data.CompletedOrders.Add(new CompletedOrderData
            {
                OrderId = id,
                CompletedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            });
            if (!session.TryConsumeItemAndGrantRewards(order.ItemId, order.Quantity, order.Coins, order.Experience))
            {
                session.Data.CompletedOrders.RemoveAt(session.Data.CompletedOrders.Count - 1);
                return false;
            }

            new TutorialService(session).TryAdvance(TutorialService.Deliver);
            new DailyTaskService(session).RecordDelivery();
            new AchievementService(session).RecordDelivery();
            session.NotifyExternalStateChanged();
            return true;
        }

        public string GetStatusSummary()
        {
            List<OrderDefinition> orders = GetOrders();
            int done = orders.Count(x => IsCompleted(x.Id));
            string[] ready = orders.Where(x => !IsCompleted(x.Id) && CanComplete(x.Id))
                .Select(x => x.DisplayName).ToArray();
            return "Orders: " + done + "/" + orders.Count + " delivered. Ready: " +
                (ready.Length > 0 ? string.Join(", ", ready) : "none") + ".";
        }

        public string FirstAvailableOrder()
        {
            foreach (OrderDefinition order in GetOrders())
                if (!IsCompleted(order.Id) && CanComplete(order.Id)) return order.Id;
            return null;
        }
    }
}