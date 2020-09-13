using CustomerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Repository
{
    public interface ICustomerRepository
    {
        Customer InsertCustomer(Customer Customer);
        Customer GetCustomer();
    }
}
