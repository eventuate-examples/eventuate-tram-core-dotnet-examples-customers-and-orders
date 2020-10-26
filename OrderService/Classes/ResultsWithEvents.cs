using OrderService.Models;
using IO.Eventuate.Tram.Events.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Classes
{
    public class ResultsWithEvents
    {
        public Order Order { get; set; }
        public List<IDomainEvent> Events { get; set; }

        public ResultsWithEvents()
        {
        }

        public ResultsWithEvents(Order order, List<IDomainEvent> events)
        {
            Order = order;
            Events = events;
        }

        public ResultsWithEvents(Order order, IDomainEvent domainEvent)
        {
            Order = order;
            List<IDomainEvent> eventList = new List<IDomainEvent>();
            eventList.Add(domainEvent);
            Events = eventList;
        }
    }
}
