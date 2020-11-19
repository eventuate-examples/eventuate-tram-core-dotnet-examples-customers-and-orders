using IO.Eventuate.Tram.Events.Subscriber;
using IO.Eventuate.Tram.Messaging.Common;
using Microsoft.Extensions.Logging;
using ServiceCommon.Classes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace OrderHistoryService.Service
{
    public class OrderHistoryEventConsumer : IDomainEventHandler<CustomerCreatedEvent>, IDomainEventHandler<OrderCreatedEvent>, IDomainEventHandler<OrderApprovedEvent>, IDomainEventHandler<OrderRejectedEvent>, IDomainEventHandler<OrderCancelledEvent>
    {
        private readonly ILogger logger;
        private OrderHistoryViewService orderHistoryViewService;
        public OrderHistoryEventConsumer(ILogger<CustomerCreatedEvent> _logger, OrderHistoryViewService _orderHistoryViewService)
        {
            logger = _logger;
            orderHistoryViewService = _orderHistoryViewService;
        }
        public void Handle(IDomainEventEnvelope<CustomerCreatedEvent> customerCreatedEvent)
        {
            orderHistoryViewService.CreateCustomer(Convert.ToInt64(customerCreatedEvent.AggregateId), customerCreatedEvent.Event.Name, customerCreatedEvent.Event.CreditLimit);
        }
        public void Handle(IDomainEventEnvelope<OrderCreatedEvent> orderCreatedEvent)
        {
            orderHistoryViewService.AddOrder(orderCreatedEvent.Event.OrderDetails.CustomerId, Convert.ToInt64(orderCreatedEvent.AggregateId), orderCreatedEvent.Event.OrderDetails.OrderTotal);
        }
        public void Handle(IDomainEventEnvelope<OrderApprovedEvent> orderApprovedEvent)
        {
            orderHistoryViewService.ApproveOrder(orderApprovedEvent.Event.OrderDetails.CustomerId, Convert.ToInt64(orderApprovedEvent.AggregateId));
        }
        public void Handle(IDomainEventEnvelope<OrderRejectedEvent> orderRejectedEvent)
        {
            orderHistoryViewService.RejectOrder(orderRejectedEvent.Event.OrderDetails.CustomerId, Convert.ToInt64(orderRejectedEvent.AggregateId));
        }
        public void Handle(IDomainEventEnvelope<OrderCancelledEvent> orderCancelledEvent)
        {
            orderHistoryViewService.CancelOrder(orderCancelledEvent.Event.OrderDetails.CustomerId, Convert.ToInt64(orderCancelledEvent.AggregateId));
        }
    }
}
