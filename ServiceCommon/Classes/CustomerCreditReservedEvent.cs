using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceCommon.Classes
{
    public class CustomerCreditReservedEvent : AbstractCustomerOrderEvent
    {
        public CustomerCreditReservedEvent(long orderId) : base(orderId)
        {
        }
        public CustomerCreditReservedEvent()
        {
        }
    }
}
