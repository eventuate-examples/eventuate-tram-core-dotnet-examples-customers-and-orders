using ServiceCommon.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCommon.Common
{
    public class GetOrderResponse
    {
        public long Id { get; set; }
        public OrderDetails OrderDetails { get; set; }
        public OrderState State { get; set; }
        public long Version { get; set; }

        public GetOrderResponse()
        {
        }

        public GetOrderResponse(long id, OrderDetails orderDetails, OrderState orderState, long version)
        {
            Id = id;
            OrderDetails = orderDetails;
            State = orderState;
            Version = version;
        }
    }
}
