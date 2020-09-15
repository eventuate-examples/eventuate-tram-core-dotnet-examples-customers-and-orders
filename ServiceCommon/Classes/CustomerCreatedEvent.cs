using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCommon.Classes
{
    public class CustomerCreatedEvent : ICustomerEvent
    {
        public String Name { get; set; }
        public Money CreditLimit { get; set; }
        public CustomerCreatedEvent()
        {
        }
        public CustomerCreatedEvent(String name, Money creditLimit)
        {
            Name = name;
            CreditLimit = creditLimit;
        }
    }
}