using System;
using System.Collections.Generic;
using MiaMax.Domain.Inventory;
using MiaMax.Domain.Progression;
using MiaMax.Domain.Farming;
using MiaMax.Domain.Production;
using MiaMax.Domain.Orders;

namespace MiaMax.Infrastructure.Save
{
    [Serializable]
    public sealed class PlayerSaveData
    {
        public const int CurrentSchemaVersion = 1;

        public int SchemaVersion = CurrentSchemaVersion;
        public long CreatedAtUnix;
        public long UpdatedAtUnix;
        public PlayerProgressData Progress = new PlayerProgressData();
        public List<ItemStackData> Inventory = new List<ItemStackData>();
        public List<FarmPlotData> FarmPlots = new List<FarmPlotData>();
        public ProductionJobData ProductionJob = new ProductionJobData();
        public List<CompletedOrderData> CompletedOrders = new List<CompletedOrderData>();
        public List<QuestProgressData> ClaimedQuests = new List<QuestProgressData>();
        public List<string> UnlockedWorldAreas = new List<string>();
    }
}
