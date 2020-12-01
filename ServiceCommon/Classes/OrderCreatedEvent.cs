using IO.Eventuate.Tram.Events.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCommon.Classes
{
    public class OrderCreatedEvent : IOrderEvent
    {
        public OrderDetails OrderDetails { get; set; }
        public OrderCreatedEvent()
        {
        }
        public OrderCreatedEvent(OrderDetails orderDetails)
        {
            OrderDetails = orderDetails;
        }

    }
}