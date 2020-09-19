using CustomerService.DBContext;
using CustomerService.Models;
using CustomerService.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceCommon.Classes;

namespace CustomerService.UnitTests.Repository
{
    [TestClass]
    public class CustomerRepositoryTests
    {
        CustomerRepository customerRepository;
        Customer customer;

        [TestInitialize]
        public void Setup()
        {
            //SetUp CustomerContext with InMemoryDatabase
            var options = new DbContextOptionsBuilder<CustomerContext>().UseInMemoryDatabase(databaseName: "TestDatabase").Options;
            var context = new CustomerContext(options);
            customerRepository = new CustomerRepository(context);
            //Initialize
            Money creditLimit = new Money("12.10");
            customer = new Customer("Joe", creditLimit);
        }
        [TestMethod]
        public void CustomerShouldbeCreated()
        {
            var response = customerRepository.InsertCustomer(customer);
            // assert
            Assert.IsNotNull(response.Id);
        }
    }
}
