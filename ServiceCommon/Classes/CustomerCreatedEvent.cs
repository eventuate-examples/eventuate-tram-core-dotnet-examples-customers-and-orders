using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCommon.Classes
{
    public class CustomerCreatedEvent : ICustomerEvent
    {
        public String Name { get; set; }
        private Money CreditLimit { get; set; }
        public CustomerCreatedEvent()
        {
        }

        public CustomerCreatedEvent(String name, Money creditLimit)
        {
            Name = name;
            CreditLimit = creditLimit;
        }

        public String GetName()
        {
            return Name;
        }

        public void SetName(String name)
        {
            Name = name;
        }

        public Money GetCreditLimit()
        {
            return CreditLimit;
        }

        public void SetCreditLimit(Money creditLimit)
        {
            CreditLimit = creditLimit;
        }
    }
}