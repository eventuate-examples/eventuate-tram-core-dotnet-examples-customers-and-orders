using ServiceCommon.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCommon.Common
{
    public class CreateCustomerRequest
    {
        public string Name { get; set; }
        public Money CreditLimit { get; set; }
    }
}
