using MongoDB.Driver;
using OrderHistoryService.Config;
using ServiceCommon.OrderHistoryCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderHistoryService.DBContext
{
    public class OrderHistoryContext : IOrderHistoryContext
    {
        private readonly IMongoDatabase _db;
        public OrderHistoryContext(MongoDBConfig config)
        {
            var client = new MongoClient(config.ConnectionString);
            _db = client.GetDatabase(config.Database);
        }
        public IMongoCollection<CustomerView> CustomerViews => _db.GetCollection<CustomerView>("CustomerViews");
    }
}
