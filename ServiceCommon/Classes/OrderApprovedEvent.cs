using IO.Eventuate.Tram.Events.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCommon.Classes
{
    public class OrderApprovedEvent : IOrderEvent
    {
        public OrderDetails OrderDetails { get; set; }
        public OrderApprovedEvent()
        {
        }
        public OrderApprovedEvent(OrderDetails orderDetails)
        {
            OrderDetails = orderDetails;
        }

    }
}