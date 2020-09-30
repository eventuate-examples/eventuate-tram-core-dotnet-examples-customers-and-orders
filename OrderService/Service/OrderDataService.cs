using OrderService.Classes;
using OrderService.Models;
using OrderService.Repository;
using IO.Eventuate.Tram.Events.Common;
using IO.Eventuate.Tram.Events.Publisher;
using ServiceCommon.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace OrderService.Service
{
    public class OrderDataService
    {
        private IOrderRepository orderRepository;
        private IDomainEventPublisher domainEventPublisher;
        public OrderDataService(IOrderRepository _orderRepository, IDomainEventPublisher _domainEventPublisher)
        {
            orderRepository = _orderRepository;
            domainEventPublisher = _domainEventPublisher;
        }
        public Order CreateOrder(OrderDetails orderDetails)
        {
            Order order;
            using (var scope = new TransactionScope())
            {
                ResultsWithEvents orderWithEvents = Create(orderDetails);
                order = orderRepository.InsertOrder(orderWithEvents.Order);
                domainEventPublisher.Publish(typeof(Order).Name, order.Id, orderWithEvents.Events);
                scope.Complete();
                return order;
            }
        }
        public static ResultsWithEvents Create(OrderDetails orderDetails)
        {
            Order order = new Order(orderDetails);
            var orderCreatedEvent = new OrderCreatedEvent(orderDetails);
            List<IDomainEvent> eventList = new List<IDomainEvent>();
            eventList.Add(orderCreatedEvent);
            return new ResultsWithEvents(order, eventList);
        }
    }
}
