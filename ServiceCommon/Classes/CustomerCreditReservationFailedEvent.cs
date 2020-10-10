using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceCommon.Classes
{
    public class CustomerCreditReservationFailedEvent : AbstractCustomerOrderEvent
    {
        public CustomerCreditReservationFailedEvent(long orderId) : base(orderId)
        {
        }
        public CustomerCreditReservationFailedEvent()
        {
        }
    }
}
