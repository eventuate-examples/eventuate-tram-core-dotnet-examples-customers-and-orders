using CustomerService.DBContext;
using CustomerService.Models;
using Microsoft.EntityFrameworkCore;
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
        public Customer Add(Customer customer)
        {
            _dbContext.Add(customer);
            _dbContext.SaveChanges();
            return customer;
        }
        public Customer FindById(long customerId)
        {
            var customer = _dbContext.Customers.Include(x=>x.CreditReservations).Where(x => x.Id == customerId).FirstOrDefault();
            return customer;
        }
        public CreditReservation Add(CreditReservation creditReservation)
        {
            _dbContext.Add(creditReservation);
            _dbContext.SaveChanges();
            return creditReservation;
        }
        public void Remove(CreditReservation creditReservation)
        {
            _dbContext.Remove(creditReservation);
            _dbContext.SaveChanges();
        }
    }
}
