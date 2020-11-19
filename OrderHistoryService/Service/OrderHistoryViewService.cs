using IO.Eventuate.Tram.Events.Common;
using IO.Eventuate.Tram.Events.Publisher;
using ServiceCommon.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using OrderHistoryService.DBContext;
using OrderHistoryService.Repository;

namespace OrderHistoryService.Service
{
    public class OrderHistoryViewService
    {
        private ICustomerViewRepository customerViewRepository;
        private IDomainEventPublisher domainEventPublisher;
        public OrderHistoryViewService(ICustomerViewRepository _customerViewRepository, IDomainEventPublisher _domainEventPublisher)
        {
            customerViewRepository = _customerViewRepository;
            domainEventPublisher = _domainEventPublisher;
        }
        public void CreateCustomer(long customerId, string customerName, Money creditLimit)
        {
            customerViewRepository.AddCustomer(customerId, customerName, creditLimit);
        }
        public void AddOrder(long customerId, long orderId, Money orderTotal)
        {
            customerViewRepository.AddOrder(customerId, orderId, orderTotal);
        }
        public void ApproveOrder(long customerId, long orderId)
        {
            customerViewRepository.UpdateOrderState(customerId, orderId, OrderState.APPROVED);
        }
        public void RejectOrder(long customerId, long orderId)
        {
            customerViewRepository.UpdateOrderState(customerId, orderId, OrderState.REJECTED);
        }
        public void CancelOrder(long customerId, long orderId)
        {
            customerViewRepository.UpdateOrderState(customerId, orderId, OrderState.CANCELLED);
        }
    }
}
