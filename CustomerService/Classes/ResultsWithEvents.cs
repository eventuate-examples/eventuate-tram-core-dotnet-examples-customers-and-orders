using CustomerService.Models;
using IO.Eventuate.Tram.Events.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Classes
{
    public class ResultsWithEvents
    {
        public Customer Customer { get; set; }
        public List<IDomainEvent> Events { get; set; }

        public ResultsWithEvents()
        {
        }

        public ResultsWithEvents(Customer customer, List<IDomainEvent> events)
        {
            Customer = customer;
            Events = events;
        }

        public ResultsWithEvents(Customer customer, IDomainEvent domainEvent)
        {
            Customer = customer;
            List<IDomainEvent> eventList = new List<IDomainEvent>();
            eventList.Add(domainEvent);
            Events = eventList;
        }
    }
}
