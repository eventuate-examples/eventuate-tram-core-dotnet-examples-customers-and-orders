using CustomerService.Classes;
using CustomerService.Models;
using CustomerService.Repository;
using IO.Eventuate.Tram.Events.Common;
using IO.Eventuate.Tram.Events.Publisher;
using Microsoft.Extensions.Logging;
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
            Customer customer = new Customer();
            using (var scope = new TransactionScope())
            {
                ResultsWithEvents customerWithEvents = customer.Create(name, creditLimit);
                customer = customerRepository.Add(customerWithEvents.Customer);
                domainEventPublisher.Publish(typeof(Customer).Name, customer.Id, customerWithEvents.Events);
                scope.Complete();
                return customer;
            }
        }
        public void ReserveCredit(long orderId, long customerId, Money orderTotal)
        {

            Customer customer = customerRepository.FindById(customerId);
            if (customer == null)
            {
                var customerValidationFailedEvent = new CustomerValidationFailedEvent(orderId);
                List<IDomainEvent> eventList = new List<IDomainEvent>();
                eventList.Add(customerValidationFailedEvent);
                domainEventPublisher.Publish(typeof(Customer).Name, customerId, eventList);
                return;
            }
            try
            {
                var creditReservation = customer.ReserveCredit(orderId, orderTotal);
                customerRepository.Add(creditReservation);
                CustomerCreditReservedEvent customerCreditReservedEvent = new CustomerCreditReservedEvent(orderId);
                List<IDomainEvent> eventList = new List<IDomainEvent>();
                eventList.Add(customerCreditReservedEvent);
                domainEventPublisher.Publish(typeof(Customer).Name, customer.Id, eventList);
            }
            catch (CustomerCreditLimitExceededException)
            {
                CustomerCreditReservationFailedEvent customerCreditReservationFailedEvent = new CustomerCreditReservationFailedEvent(orderId);
                List<IDomainEvent> eventList = new List<IDomainEvent>();
                eventList.Add(customerCreditReservationFailedEvent);
                domainEventPublisher.Publish(typeof(Customer).Name, customer.Id, eventList);
            }
        }
        public void ReleaseCredit(long orderId, long customerId)
        {
            Customer customer = customerRepository.FindById(customerId);
            if (customer == null)
            {
                var customerValidationFailedEvent = new CustomerValidationFailedEvent(orderId);
                List<IDomainEvent> eventList = new List<IDomainEvent>();
                eventList.Add(customerValidationFailedEvent);
                domainEventPublisher.Publish(typeof(Customer).Name, customerId, eventList);
                return;
            }
            var creditReservation = customer.CreditReservations.Where(o => o.OrderId == orderId).FirstOrDefault();
            customerRepository.Remove(creditReservation);
        }
    }
}
