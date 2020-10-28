using IO.Eventuate.Tram.Events.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCommon.Classes
{
    public class OrderCancelledEvent : IDomainEvent
    {
        public OrderDetails OrderDetails { get; set; }
        public OrderCancelledEvent()
        {
        }
        public OrderCancelledEvent(OrderDetails orderDetails)
        {
            OrderDetails = orderDetails;
        }

    }
}