using ServiceCommon.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCommon.OrderHistoryCommon
{
    public class OrderInfo
    {
        public OrderState State { get; set; }
        public long OrderId { get; set; }
        public Money OrderTotal { get; set; }
        public OrderInfo()
        {
        }

        public OrderInfo(long _orderId, Money _orderTotal)
        {
            OrderId = _orderId;
            OrderTotal = _orderTotal;
            State = OrderState.PENDING;
        }
    }
}
