using CustomerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Repository
{
    public interface ICustomerRepository
    {
        Customer Add(Customer Customer);
        Customer FindById(long customerId);
        CreditReservation Add(CreditReservation creditReservation);
        void Remove(CreditReservation creditReservation);
    }
}
