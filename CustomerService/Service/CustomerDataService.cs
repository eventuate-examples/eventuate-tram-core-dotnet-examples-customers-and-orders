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
            using (var scope = new TransactionScope())
            {
                if (customer == null)
                {
                    List<IDomainEvent> eventList = new List<IDomainEvent>();
                    eventList.Add(new CustomerValidationFailedEvent(orderId));
                    domainEventPublisher.Publish(typeof(Customer).Name, customerId, eventList);
                }
                else
                {
                    try
                    {
                        var creditReservation = customer.ReserveCredit(orderId, orderTotal);
                        customerRepository.Add(creditReservation);
                        List<IDomainEvent> eventList = new List<IDomainEvent>();
                        eventList.Add(new CustomerCreditReservedEvent(orderId));
                        domainEventPublisher.Publish(typeof(Customer).Name, customer.Id, eventList);
                    }
                    catch (CustomerCreditLimitExceededException)
                    {
                        List<IDomainEvent> eventList = new List<IDomainEvent>();
                        eventList.Add(new CustomerCreditReservationFailedEvent(orderId));
                        domainEventPublisher.Publish(typeof(Customer).Name, customer.Id, eventList);
                    }
                }
                scope.Complete();
            }
        }
        public void ReleaseCredit(long orderId, long customerId)
        {
            Customer customer = customerRepository.FindById(customerId);
            using (var scope = new TransactionScope())
            {
                if (customer == null)
                {
                    List<IDomainEvent> eventList = new List<IDomainEvent>();
                    eventList.Add(new CustomerValidationFailedEvent(orderId));
                    domainEventPublisher.Publish(typeof(Customer).Name, customerId, eventList);
                }
                else
                {
                    var creditReservation = customer.CreditReservations.Where(o => o.OrderId == orderId).FirstOrDefault();
                    customerRepository.Remove(creditReservation);
                }
                scope.Complete();
            }
        }
    }
}
