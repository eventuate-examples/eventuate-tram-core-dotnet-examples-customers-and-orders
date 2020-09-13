using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCommon.Classes
{
    public class CustomerCreatedEvent : ICustomerEvent
    {
        public String name { get; set; }
        private Money creditLimit { get; set; }
        public CustomerCreatedEvent()
        {
        }

        public CustomerCreatedEvent(String _name, Money _creditLimit)
        {
            name = name;
            creditLimit = creditLimit;
        }

        public String GetName()
        {
            return name;
        }

        public void SetName(String name)
        {
            this.name = name;
        }

        public Money GetCreditLimit()
        {
            return creditLimit;
        }

        public void SetCreditLimit(Money _creditLimit)
        {
            creditLimit = _creditLimit;
        }
    }
}