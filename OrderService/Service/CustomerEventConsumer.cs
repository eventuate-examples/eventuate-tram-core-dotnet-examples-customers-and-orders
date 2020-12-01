using IO.Eventuate.Tram.Events.Subscriber;
using IO.Eventuate.Tram.Messaging.Common;
using Microsoft.Extensions.Logging;
using ServiceCommon.Classes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Service
{
    public class CustomerEventConsumer : IDomainEventHandler<CustomerCreditReservedEvent>, IDomainEventHandler<CustomerValidationFailedEvent>, IDomainEventHandler<CustomerCreditReservationFailedEvent>
    {
        private readonly ILogger logger;
        private OrderService orderService;
        public CustomerEventConsumer(ILogger<CustomerCreatedEvent> _logger, OrderService _orderService)
        {
            logger = _logger;
            orderService = _orderService;
        }
        public void Handle(IDomainEventEnvelope<CustomerCreditReservedEvent> customerCreditReservedEvent)
        {
            orderService.ApproveOrder(customerCreditReservedEvent.Event.orderId);
        }
        public void Handle(IDomainEventEnvelope<CustomerValidationFailedEvent> customerValidationFailedEvent)
        {
            orderService.RejectOrder(customerValidationFailedEvent.Event.orderId);
        }
        public void Handle(IDomainEventEnvelope<CustomerCreditReservationFailedEvent> customerCreditReservationFailedEvent)
        {
            orderService.RejectOrder(customerCreditReservationFailedEvent.Event.orderId);
        }
    }
}
