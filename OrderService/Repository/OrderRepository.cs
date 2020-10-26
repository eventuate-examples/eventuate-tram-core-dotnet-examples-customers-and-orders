using OrderService.DBContext;
using OrderService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderContext _dbContext;
        public OrderRepository(OrderContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Order Add(Order order)
        {
            _dbContext.Add(order);
            _dbContext.SaveChanges();
            return order;
        }
        public Order FindById(long orderId)
        {
            var customer = _dbContext.Orders.Where(x => x.Id == orderId).FirstOrDefault();
            return customer;
        }
        public Order Update(Order order)
        {
            _dbContext.SaveChanges();
            return order;
        }
    }
}
