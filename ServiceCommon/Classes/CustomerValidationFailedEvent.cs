using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceCommon.Classes
{
    public class CustomerValidationFailedEvent : AbstractCustomerOrderEvent
    {
        public CustomerValidationFailedEvent(long orderId) : base(orderId)
        {
        }
        public CustomerValidationFailedEvent()
        {
        }
    }
}
