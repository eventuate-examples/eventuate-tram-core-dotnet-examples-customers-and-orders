using CustomerService.DBContext;
using CustomerService.Models;
using CustomerService.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceCommon.Classes;
using System;
using System.Collections.Generic;
using System.Text;

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
            //SetUp CustomerContext
            var options = new DbContextOptionsBuilder<CustomerContext>().UseInMemoryDatabase(databaseName: "TestDatabase").Options;
            var context = new CustomerContext(options);
            customerRepository = new CustomerRepository(context);
            //Initialize DB object
            Money creditLimit = new Money("12.10");
            customer = new Customer("Joe", creditLimit);
        }
        [TestMethod]
        public void CustomerShouldbeCreated()
        {
            var result = customerRepository.InsertCustomer(customer);
            // assert
            Assert.AreEqual(1, result.Id);
        }
    }
}
