using IO.Eventuate.Tram.Events.Subscriber;
using IO.Eventuate.Tram.Messaging.Common;
using Microsoft.Extensions.Logging;
using Nest;
using OrderHistoryTextSearchService.Controllers;
using ServiceCommon.Classes;
using ServiceCommon.OrderHistoryTextSearchCommon;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace OrderHistoryTextSearchService.Service
{
    public class OrderHistoryTextSearchEventConsumer : IDomainEventHandler<CustomerCreatedEvent>, IDomainEventHandler<OrderCreatedEvent>
    {
        public TextViewService<CustomerTextView> customerTextViewService;
        public TextViewService<OrderTextView> orderTextViewService;
        private readonly IElasticClient _elasticClient;
        public OrderHistoryTextSearchEventConsumer(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
            customerTextViewService = new TextViewService<CustomerTextView>(_elasticClient, CustomerTextView.INDEX, CustomerTextView.TYPE);
            orderTextViewService = new TextViewService<OrderTextView>(_elasticClient, OrderTextView.INDEX, OrderTextView.TYPE);
        }
        public void Handle(IDomainEventEnvelope<CustomerCreatedEvent> customerCreatedEvent)
        {
            var customerTextView = new CustomerTextView
            {
                id = customerCreatedEvent.AggregateId,
                Name = customerCreatedEvent.Event.Name,
                CreditLimit = customerCreatedEvent.Event.CreditLimit.Amount.ToString()
            };
            customerTextViewService.Index(customerTextView);
        }
        public void Handle(IDomainEventEnvelope<OrderCreatedEvent> orderCreatedEvent)
        {      
            var orderTextView = new OrderTextView
            {
                id = orderCreatedEvent.AggregateId,
                CustomerId = orderCreatedEvent.Event.OrderDetails.CustomerId.ToString(),
                OrderTotal = orderCreatedEvent.Event.OrderDetails.OrderTotal.Amount.ToString(),
                State = "PENDING",
            };
            orderTextViewService.Index(orderTextView);
        }
    }
}
