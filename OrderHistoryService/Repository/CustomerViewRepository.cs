using MongoDB.Bson;
using MongoDB.Driver;
using OrderHistoryService.DBContext;
using ServiceCommon.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceCommon.OrderHistoryCommon;

namespace OrderHistoryService.Repository
{
    public class CustomerViewRepository : ICustomerViewRepository
    {
        private readonly IOrderHistoryContext _context;
        public CustomerViewRepository(IOrderHistoryContext context)
        {
            _context = context;
        }
        public IEnumerable<CustomerView> GetAll()
        {
            return _context.CustomerViews.Find(_ => true).ToList();
        }
        public long Count()
        {
            return _context.CustomerViews.Find(_ => true).CountDocuments();
        }
        public CustomerView FindById(long id)
        {
            return _context.CustomerViews.Find(m => m.Id == id).FirstOrDefault();
        }
        public void AddCustomer(long customerId, string customerName, Money creditLimit)
        {
            var update = Builders<CustomerView>.Update.Set("Id", customerId).Set("Name", customerName).Set("CreditLimit", creditLimit);
            _context.CustomerViews.UpdateOne(m => m.Id == customerId, update, new UpdateOptions { IsUpsert = true });
        }
        public void AddOrder(long customerId, long orderId, Money orderTotal)
        {
            var update = Builders<CustomerView>.Update.Set("Orders." + orderId, new OrderInfo { OrderId = orderId, OrderTotal = orderTotal });
            _context.CustomerViews.UpdateOne(m => m.Id == customerId, update, new UpdateOptions { IsUpsert = true });
        }
        public void UpdateOrderState(long customerId, long orderId, OrderState state)
        {
            var update = Builders<CustomerView>.Update.Set("Orders." + orderId + ".State", state);
            _context.CustomerViews.UpdateOne(m => m.Id == customerId, update, new UpdateOptions { IsUpsert = true });
        }
    }
}
