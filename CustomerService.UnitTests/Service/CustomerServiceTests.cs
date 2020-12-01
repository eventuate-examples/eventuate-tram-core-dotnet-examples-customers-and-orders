using CustomerService.DBContext;
using CustomerService.Models;
using CustomerService.Repository;
using CustomerService.Service;
using IO.Eventuate.Tram;
using IO.Eventuate.Tram.Events.Publisher;
using IO.Eventuate.Tram.Events.Subscriber;
using IO.Eventuate.Tram.Local.Kafka.Consumer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceCommon.Classes;
using ServiceCommon.Helpers;
using System;

namespace CustomerService.UnitTests.Service
{
    [TestClass]
    public class CustomerServiceTests
    {
        CustomerService.Service.CustomerService customerService;
        Money creditLimit;
        IDomainEventPublisher domainEventPublisher;
        TestEventConsumer consumer;

        [TestInitialize]
        public void Setup()
        {
            var host = new HostBuilder()
         .ConfigureServices((hostContext, services) =>
         {
             //DbContext
             services.AddDbContext<CustomerContext>(o => o.UseSqlServer(TestSettings.EventuateDB));
             // Kafka Transport
             services.AddEventuateTramSqlKafkaTransport(TestSettings.EventuateTramDbSchema, TestSettings.KafkaBootstrapServers, EventuateKafkaConsumerConfigurationProperties.Empty(),
                (provider, o) =>
                {
                    var applicationDbContext = provider.GetRequiredService<CustomerContext>();
                    //applicationDbContext.Database.Migrate();
                    o.UseSqlServer(applicationDbContext.Database.GetDbConnection());
                });
             // Publisher
             services.AddEventuateTramEventsPublisher();
             // Consumer
             services.AddSingleton<TestEventConsumer>();
             // Dispatcher
             services.AddEventuateTramDomainEventDispatcher(Guid.NewGuid().ToString(),
             provider => DomainEventHandlersBuilder.ForAggregateType(typeof(Customer).Name)
              .OnEvent<CustomerCreatedEvent, TestEventConsumer>()
              .OnEvent<CustomerValidationFailedEvent, TestEventConsumer>()
              .OnEvent<CustomerCreditReservedEvent, TestEventConsumer>()
              .OnEvent<CustomerCreditReservationFailedEvent, TestEventConsumer>()
              .Build());
             // Repository
             services.AddTransient<ICustomerRepository, CustomerRepository>();
         }).Build();
            host.StartAsync().Wait();

            //Services
            domainEventPublisher = host.Services.GetService<IDomainEventPublisher>();
            consumer = host.Services.GetService<TestEventConsumer>();
            var customerRepository = host.Services.GetService<ICustomerRepository>();
            customerService = new CustomerService.Service.CustomerService(customerRepository, domainEventPublisher);
            //Initialize Money
            creditLimit = new Money("12.10");
        }

        [TestMethod]
        public void ReserveCreditAndEventShouldBePublished()
        {
            //Create Customer
            long customerId = CreateCustomer();
            //Reserve Credit
            Money orderTotal = new Money("5.10");
            customerService.ReserveCredit(System.DateTime.Now.Ticks, customerId, orderTotal);
            AssertEvent<CustomerCreditReservedEvent>();
        }
        [TestMethod]
        public void CreditLimitExceededExceptionEventShouldBePublished()
        {
            //Create Customer
            long customerId = CreateCustomer();
            //Reserve Credit with OrderTotal more than Customer's CreditLimit 
            Money orderTotal = new Money("20.10");
            customerService.ReserveCredit(System.DateTime.Now.Ticks, customerId, orderTotal);
            AssertEvent<CustomerCreditReservationFailedEvent>();

        }

        [TestMethod]
        public void CustomerValidationFailedEventShouldBePublished()
        {
            //Reserve Credit with random CustomerId
            Money orderTotal = new Money("20.10");
            customerService.ReserveCredit(System.DateTime.Now.Ticks, System.DateTime.Now.Ticks, orderTotal);
            AssertEvent<CustomerValidationFailedEvent>();
        }

        [TestMethod]
        public void ReleaseCreditAndEventShouldBePublished()
        {
            //Create Customer
            long customerId = CreateCustomer();
            //Reserve Credit
            Money orderTotal = new Money("10.10");
            long orderId = System.DateTime.Now.Ticks;
            customerService.ReserveCredit(orderId, customerId, orderTotal);
            AssertEvent<CustomerCreditReservedEvent>();

            //Release Credit
            customerService.ReleaseCredit(orderId, customerId);            

            //Reserve Credit again
            customerService.ReserveCredit(System.DateTime.Now.Ticks, customerId, orderTotal);
            AssertEvent<CustomerCreditReservedEvent>();

        }

        private long CreateCustomer()
        {
            var customer = customerService.CreateCustomer("Joe", creditLimit);
            AssertEvent<CustomerCreatedEvent>();
            //assert
            Assert.AreEqual("Joe", customer.Name);
            return customer.Id;
        }
        private void AssertEvent<T>()
        {
            Util.Eventually(50, 1000, () =>
            {
                var eventList = consumer.GetEvents();
                if (eventList.Count == 0)
                {
                    throw new System.InvalidOperationException("Event not found");
                }
                foreach (var domainEvent in eventList)
                {
                    try
                    {
                        T customerEvent = (T)domainEvent;
                        //Assert
                        Assert.IsNotNull(customerEvent);
                        consumer.RemoveEvent(domainEvent);
                        break;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            });
        }
    }
}
