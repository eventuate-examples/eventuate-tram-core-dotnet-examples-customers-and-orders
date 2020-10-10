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
                order = orderRepository.Add(orderWithEvents.Order);
                domainEventPublisher.Publish(typeof(Order).Name, order.Id, orderWithEvents.Events);
                scope.Complete();
                return order;
            }
        }
        public void ApproveOrder(long orderId)
        {
            Order order = orderRepository.FindById(orderId);
            if (order == null)
            {
                throw new System.ArgumentException(string.Format("Order with id {0} not found", orderId));
            }
            order.State = OrderState.APPROVED;
            orderRepository.Update(order);
            List<IDomainEvent> eventList = new List<IDomainEvent>();
            var orderApprovedEvent = new OrderApprovedEvent(order.OrderDetails);
            eventList.Add(orderApprovedEvent);
            domainEventPublisher.Publish(typeof(Order).Name, order.Id, eventList);
        }
        public void RejectOrder(long orderId)
        {
            Order order = orderRepository.FindById(orderId);
            if (order == null)
            {
                throw new System.ArgumentException(string.Format("Order with id {0} not found", orderId));
            }
            order.State = OrderState.REJECTED;
            orderRepository.Update(order);
            List<IDomainEvent> eventList = new List<IDomainEvent>();
            var orderRejectedEvent = new OrderRejectedEvent(order.OrderDetails);
            eventList.Add(orderRejectedEvent);
            domainEventPublisher.Publish(typeof(Order).Name, order.Id, eventList);
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
