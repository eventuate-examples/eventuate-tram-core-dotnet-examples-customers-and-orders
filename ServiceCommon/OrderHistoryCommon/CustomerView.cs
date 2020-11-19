using ServiceCommon.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCommon.OrderHistoryCommon
{
    public class CustomerView
    {
        public long Id { get; set; }
        public Dictionary<long, OrderInfo> Orders { get; set; }
        public string Name { get; set; }
        public Money CreditLimit { get; set; }
    }
}
