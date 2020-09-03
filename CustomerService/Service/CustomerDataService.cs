using CustomerService.Classes;
using CustomerService.Models;
using CustomerService.Repository;
using IO.Eventuate.Tram.Events.Publisher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Service
{
    public class CustomerDataService
    {
        private ICustomerRepository customerRepository;
        private IDomainEventPublisher domainEventPublisher;
        public CustomerDataService(ICustomerRepository _customerRepository, IDomainEventPublisher _domainEventPublisher)
        {
            customerRepository = _customerRepository;
            domainEventPublisher = _domainEventPublisher;
        }
        public Customer CreateCustomer(String name, decimal creditLimit)
        {
            ResultsWithEvents customerWithEvents = Customer.Create(name, creditLimit);
            Customer customer = customerRepository.InsertCustomer(customerWithEvents.customer);
            domainEventPublisher.Publish(customer.id.ToString(), customer.id, customerWithEvents.events);
            return customer;
        }
    }
}
