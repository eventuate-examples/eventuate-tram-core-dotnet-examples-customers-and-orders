using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceCommon.OrderHistoryCommon;

namespace OrderHistoryService.DBContext
{
    public interface IOrderHistoryContext
    {
        IMongoCollection<CustomerView> CustomerViews { get; }
    }
}
