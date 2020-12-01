using IO.Eventuate.Tram.Events.Common;
using IO.Eventuate.Tram.Events.Subscriber;
using IO.Eventuate.Tram.Messaging.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ServiceCommon.Classes
{
    public class TestEventConsumer : IDomainEventHandler<CustomerCreatedEvent>, IDomainEventHandler<CustomerValidationFailedEvent>, IDomainEventHandler<CustomerCreditReservedEvent>, IDomainEventHandler<CustomerCreditReservationFailedEvent>, IDomainEventHandler<OrderCreatedEvent>, IDomainEventHandler<OrderCancelledEvent>, IDomainEventHandler<OrderApprovedEvent>, IDomainEventHandler<OrderRejectedEvent>
    {
        private List<IDomainEvent> events = new List<IDomainEvent>();
        public void Handle(IDomainEventEnvelope<CustomerCreatedEvent> customerCreatedEvent)
        {
            events.Add(customerCreatedEvent.Event);
        }
        public void Handle(IDomainEventEnvelope<CustomerValidationFailedEvent> customerValidationFailedEvent)
        {
            events.Add(customerValidationFailedEvent.Event);
        }
        public void Handle(IDomainEventEnvelope<CustomerCreditReservedEvent> customerCreditReservedEvent)
        {
            events.Add(customerCreditReservedEvent.Event);
        }
        public void Handle(IDomainEventEnvelope<CustomerCreditReservationFailedEvent> customerCreditReservationFailedEvent)
        {
            events.Add(customerCreditReservationFailedEvent.Event);
        }
        public void Handle(IDomainEventEnvelope<OrderCreatedEvent> orderCreatedEvent)
        {
            events.Add(orderCreatedEvent.Event);
        }
        public void Handle(IDomainEventEnvelope<OrderCancelledEvent> orderCancelledEvent)
        {
            events.Add(orderCancelledEvent.Event);
        }
        public void Handle(IDomainEventEnvelope<OrderApprovedEvent> OrderApprovedEvent)
        {
            events.Add(OrderApprovedEvent.Event);
        }
        public void Handle(IDomainEventEnvelope<OrderRejectedEvent> orderRejectedEvent)
        {
            events.Add(orderRejectedEvent.Event);
        }
        public List<IDomainEvent> GetEvents()
        {
            return events;
        }
        public void RemoveEvent(IDomainEvent domainEvent)
        {
            events.Remove(domainEvent);
        }
    }
}
