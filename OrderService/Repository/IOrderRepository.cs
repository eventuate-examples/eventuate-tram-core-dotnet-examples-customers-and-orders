using OrderService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Repository
{
    public interface IOrderRepository
    {
        Order InsertOrder(Order order);
    }
}
