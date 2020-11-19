using ServiceCommon.OrderHistoryCommon;
using ServiceCommon.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderHistoryService.Repository
{
    public interface ICustomerViewRepository
    {
        IEnumerable<CustomerView> GetAll();
        CustomerView FindById(long id);
        long Count();
        void AddCustomer(long customerId, string customerName, Money creditLimit);
        void AddOrder(long customerId, long orderId, Money orderTotal);
        void UpdateOrderState(long customerId, long orderId, OrderState state);
    }
}
