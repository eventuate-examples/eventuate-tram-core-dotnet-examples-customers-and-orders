using IO.Eventuate.Tram.Events.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCommon.Classes
{
    public class OrderRejectedEvent : IDomainEvent
    {
        public OrderDetails OrderDetails { get; set; }
        public OrderRejectedEvent()
        {
        }
        public OrderRejectedEvent(OrderDetails orderDetails)
        {
            OrderDetails = orderDetails;
        }

    }
}