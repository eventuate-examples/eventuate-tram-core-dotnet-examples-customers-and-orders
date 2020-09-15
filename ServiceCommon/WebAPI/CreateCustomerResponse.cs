using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCommon.Common
{
    public class CreateCustomerResponse
    {
        public long CustomerId { get; set; }

        public CreateCustomerResponse()
        {
        }

        public CreateCustomerResponse(long customerId)
        {
            CustomerId = customerId;
        }
    }
}
