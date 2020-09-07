using CustomerService.DBContext;
using CustomerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerContext _dbContext;
        public CustomerRepository(CustomerContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Customer InsertCustomer(Customer customer)
        {
            _dbContext.Add(customer);
            _dbContext.SaveChanges();
            return customer;
        }
        public Customer GetCustomer()
        {
            var cust = _dbContext.Customers.FirstOrDefault();
            return cust;
        }
    }
}
