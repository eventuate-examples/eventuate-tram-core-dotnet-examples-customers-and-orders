using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCommon.Common
{
    public class CreateOrderResponse
    {
        public long OrderId { get; set; }

        public CreateOrderResponse()
        {
        }

        public CreateOrderResponse(long orderId)
        {
            OrderId = orderId;
        }
    }
}
