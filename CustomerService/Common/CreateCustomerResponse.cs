using CustomerService.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Common
{
    public class CreateCustomerResponse
    {
        public long customerId { get; set; }

        public CreateCustomerResponse()
        {
        }

        public CreateCustomerResponse(long _customerId)
        {
            customerId = _customerId;
        }
    }
}
