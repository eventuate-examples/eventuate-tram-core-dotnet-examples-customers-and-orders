using CustomerService.Classes;
using IO.Eventuate.Tram.Events.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Models
{
    public class Customer
    {
        public long id { get; set; }
        public string name { get; set; }

        private decimal creditLimit;

        //private Dictionary<long, Money> creditReservations;

        private DateTime creationTime;

        public Customer()
        {
        }

        public Customer(String _name, decimal _creditLimit)
        {
            name = _name;
            creditLimit = _creditLimit;
            creationTime = System.DateTime.Now;
        }

        public static ResultsWithEvents Create(String name, decimal creditLimit)
        {
            Customer customer = new Customer(name, creditLimit);
            var customerCreatedEvent = new CustomerCreatedEvent(customer.name, customer.creditLimit);
            List<IDomainEvent> eventList = new List<IDomainEvent>();
            eventList.Add(customerCreatedEvent);
            return new ResultsWithEvents(customer, eventList);
        }
    }
}
