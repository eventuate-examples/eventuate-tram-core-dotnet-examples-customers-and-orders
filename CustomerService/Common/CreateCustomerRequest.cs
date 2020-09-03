using CustomerService.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Common
{
    public class CreateCustomerRequest
    {
        public string Name { get; set; }
        public decimal CreditLimit { get; set; }
    }
}
