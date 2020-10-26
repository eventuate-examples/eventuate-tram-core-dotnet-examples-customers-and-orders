using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceCommon.Classes
{
    public abstract class AbstractCustomerOrderEvent : ICustomerEvent
    {
        public long orderId { get; set; }

        public AbstractCustomerOrderEvent(long _orderId)
        {
            orderId = _orderId;
        }

        public AbstractCustomerOrderEvent()
        {
        }
    }
}
