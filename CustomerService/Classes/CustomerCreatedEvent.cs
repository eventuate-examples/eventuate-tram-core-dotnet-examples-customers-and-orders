using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Classes
{
    public class CustomerCreatedEvent : ICustomerEvent
    {
        private String name;
        private decimal creditLimit;
        public CustomerCreatedEvent()
        {
        }

        public CustomerCreatedEvent(String name, decimal creditLimit)
        {
            this.name = name;
            this.creditLimit = creditLimit;
        }

        public String GetName()
        {
            return name;
        }

        public void SetName(String name)
        {
            this.name = name;
        }

        public decimal GetCreditLimit()
        {
            return creditLimit;
        }

        public void SetCreditLimit(decimal creditLimit)
        {
            this.creditLimit = creditLimit;
        }
    }
}