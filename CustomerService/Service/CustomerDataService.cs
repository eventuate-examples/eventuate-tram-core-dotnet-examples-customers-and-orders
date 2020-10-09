using CustomerService.Classes;
using CustomerService.Models;
using CustomerService.Repository;
using IO.Eventuate.Tram.Events.Common;
using IO.Eventuate.Tram.Events.Publisher;
using ServiceCommon.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

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
        public Customer CreateCustomer(String name, Money creditLimit)
        {
            Customer customer;
            using (var scope = new TransactionScope())
            {
                ResultsWithEvents customerWithEvents = Create(name, creditLimit);
                customer = customerRepository.InsertCustomer(customerWithEvents.Customer);
                domainEventPublisher.Publish(typeof(Customer).Name, customer.Id, customerWithEvents.Events);
                scope.Complete();
                return customer;
            }
        }
        public static ResultsWithEvents Create(String name, Money creditLimit)
        {
            Customer customer = new Customer(name, creditLimit);
            var customerCreatedEvent = new CustomerCreatedEvent(customer.Name, customer.CreditLimit);
            List<IDomainEvent> eventList = new List<IDomainEvent>();
            eventList.Add(customerCreatedEvent);
            return new ResultsWithEvents(customer, eventList);
        }
    }
}
