using System;
namespace MiaMax.Domain.Orders
{
    [Serializable]
    public sealed class CompletedOrderData
    {
        public string OrderId;
        public long CompletedAtUnix;
    }
}