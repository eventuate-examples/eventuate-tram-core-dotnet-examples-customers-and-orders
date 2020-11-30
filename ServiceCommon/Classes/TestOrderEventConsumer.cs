using IO.Eventuate.Tram.Events.Subscriber;
using IO.Eventuate.Tram.Messaging.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ServiceCommon.Classes
{
    public class TestOrderEventConsumer : IDomainEventHandler<OrderCreatedEvent>, IDomainEventHandler<OrderCancelledEvent>, IDomainEventHandler<OrderApprovedEvent>, IDomainEventHandler<OrderRejectedEvent>
    {
        private BlockingCollection<OrderCreatedEvent> queueOrderCreatedEvent = new BlockingCollection<OrderCreatedEvent>();
        private BlockingCollection<OrderCancelledEvent> queueOrderCancelledEvent = new BlockingCollection<OrderCancelledEvent>();
        private BlockingCollection<OrderApprovedEvent> queueOrderApprovedEvent = new BlockingCollection<OrderApprovedEvent>();
        private BlockingCollection<OrderRejectedEvent> queueOrderRejectedEvent = new BlockingCollection<OrderRejectedEvent>();

        public void Handle(IDomainEventEnvelope<OrderCreatedEvent> orderCreatedEvent)
        {
            queueOrderCreatedEvent.Add(orderCreatedEvent.Event);
        }
        public void Handle(IDomainEventEnvelope<OrderCancelledEvent> orderCancelledEvent)
        {
            queueOrderCancelledEvent.Add(orderCancelledEvent.Event);
        }
        public void Handle(IDomainEventEnvelope<OrderApprovedEvent> OrderApprovedEvent)
        {
            queueOrderApprovedEvent.Add(OrderApprovedEvent.Event);
        }
        public void Handle(IDomainEventEnvelope<OrderRejectedEvent> orderRejectedEvent)
        {
            queueOrderRejectedEvent.Add(orderRejectedEvent.Event);
        }

        public BlockingCollection<OrderCreatedEvent> GetOrderCreatedEventQueue()
        {
            return queueOrderCreatedEvent;
        }
        public BlockingCollection<OrderCancelledEvent> GetOrderCancelledEventQueue()
        {
            return queueOrderCancelledEvent;
        }
        public BlockingCollection<OrderApprovedEvent> GetOrderApprovedEventQueue()
        {
            return queueOrderApprovedEvent;
        }
        public BlockingCollection<OrderRejectedEvent> GetOrderRejectedEventQueue()
        {
            return queueOrderRejectedEvent;
        }

    }
}
