using ServiceCommon.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCommon.Common
{
    public class CreateOrderRequest
    {
        public long CustomerId { get; set; }
        public Money OrderTotal { get; set; }
    }
}
