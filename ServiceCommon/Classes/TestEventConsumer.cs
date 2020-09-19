using IO.Eventuate.Tram.Events.Subscriber;
using IO.Eventuate.Tram.Messaging.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ServiceCommon.Classes
{
    public class TestEventConsumer
    {
        private BlockingCollection<CustomerCreatedEvent> queue = new BlockingCollection<CustomerCreatedEvent>();
        public TestEventConsumer()
        {
        }
        public DomainEventHandlers DomainEventHandlers(string aggregateType)
        {
            return DomainEventHandlersBuilder.ForAggregateType(aggregateType)
                .OnEvent<CustomerCreatedEvent>(HandleCustomerCreatedEvent)
                .Build();
        }

        private void HandleCustomerCreatedEvent(IDomainEventEnvelope<CustomerCreatedEvent> customerCreatedEvent)
        {
            queue.Add(customerCreatedEvent.Event);
        }
        public BlockingCollection<CustomerCreatedEvent> GetQueue()
        {
            return queue;
        }
    }
}
