using IO.Eventuate.Tram.Events.Subscriber;
using IO.Eventuate.Tram.Messaging.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ServiceCommon.Classes
{
    public class TestCustomerEventConsumer : IDomainEventHandler<CustomerCreatedEvent>, IDomainEventHandler<CustomerValidationFailedEvent>, IDomainEventHandler<CustomerCreditReservedEvent>, IDomainEventHandler<CustomerCreditReservationFailedEvent>
    {
        private BlockingCollection<CustomerCreatedEvent> queueCustomerCreatedEvent = new BlockingCollection<CustomerCreatedEvent>();
        private BlockingCollection<CustomerValidationFailedEvent> queueCustomerValidationFailedEvent = new BlockingCollection<CustomerValidationFailedEvent>();
        private BlockingCollection<CustomerCreditReservedEvent> queueCustomerCreditReservedEvent = new BlockingCollection<CustomerCreditReservedEvent>();
        private BlockingCollection<CustomerCreditReservationFailedEvent> queueCustomerCreditReservationFailedEvent = new BlockingCollection<CustomerCreditReservationFailedEvent>();

        public void Handle(IDomainEventEnvelope<CustomerCreatedEvent> customerCreatedEvent)
        {
            queueCustomerCreatedEvent.Add(customerCreatedEvent.Event);
        }
        public void Handle(IDomainEventEnvelope<CustomerValidationFailedEvent> customerValidationFailedEvent)
        {
            queueCustomerValidationFailedEvent.Add(customerValidationFailedEvent.Event);
        }
        public void Handle(IDomainEventEnvelope<CustomerCreditReservedEvent> customerCreditReservedEvent)
        {
            queueCustomerCreditReservedEvent.Add(customerCreditReservedEvent.Event);
        }
        public void Handle(IDomainEventEnvelope<CustomerCreditReservationFailedEvent> customerCreditReservationFailedEvent)
        {
            queueCustomerCreditReservationFailedEvent.Add(customerCreditReservationFailedEvent.Event);
        }

        public BlockingCollection<CustomerCreatedEvent> GetCustomerCreatedEventQueue()
        {
            return queueCustomerCreatedEvent;
        }
        public BlockingCollection<CustomerValidationFailedEvent> GetCustomerValidationFailedEventQueue()
        {
            return queueCustomerValidationFailedEvent;
        }
        public BlockingCollection<CustomerCreditReservedEvent> GetCustomerCreditReservedEventQueue()
        {
            return queueCustomerCreditReservedEvent;
        }
        public BlockingCollection<CustomerCreditReservationFailedEvent> GetCustomerCreditReservationFailedEventQueue()
        {
            return queueCustomerCreditReservationFailedEvent;
        }
    }
}
