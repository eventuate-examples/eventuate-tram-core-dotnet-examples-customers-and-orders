using IO.Eventuate.Tram.Events.Subscriber;
using IO.Eventuate.Tram.Messaging.Common;
using Microsoft.Extensions.Logging;
using ServiceCommon.Classes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CustomerService.Service
{
    public class OrderEventConsumer : IDomainEventHandler<OrderCreatedEvent>
    {
        private readonly ILogger logger;
        private CustomerDataService customerService;
        public OrderEventConsumer(ILogger<CustomerCreatedEvent> _logger, CustomerDataService _customerService)
        {
            logger = _logger;
            customerService = _customerService;
        }
        public void Handle(IDomainEventEnvelope<OrderCreatedEvent> orderCreatedEvent)
        {
            customerService.ReserveCredit(Convert.ToInt32(orderCreatedEvent.AggregateId), orderCreatedEvent.Event.OrderDetails.CustomerId, orderCreatedEvent.Event.OrderDetails.OrderTotal);
        }

    }
}
