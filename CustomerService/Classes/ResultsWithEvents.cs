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
        public Customer customer;
        public List<IDomainEvent> events;

        public ResultsWithEvents()
        {
        }

        public ResultsWithEvents(Customer _customer, List<IDomainEvent> _events)
        {
            customer = _customer;
            events = _events;
        }

        public ResultsWithEvents(Customer _customer, IDomainEvent _event)
        {
            customer = _customer;
            List<IDomainEvent> eventList = new List<IDomainEvent>();
            eventList.Add(_event);
            events = eventList;
        }
    }
}
