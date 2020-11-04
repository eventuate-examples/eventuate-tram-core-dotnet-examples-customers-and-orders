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
using System;

namespace CustomerService.UnitTests.Service
{
    [TestClass]
    public class CustomerServiceTests
    {
        CustomerDataService customerDataService;
        Money creditLimit;
        IDomainEventPublisher domainEventPublisher;
        TestCustomerEventConsumer consumer;

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
                    applicationDbContext.Database.Migrate();
                    o.UseSqlServer(applicationDbContext.Database.GetDbConnection());
                });
             // Publisher
             services.AddEventuateTramEventsPublisher();
             // Consumer
             services.AddSingleton<TestCustomerEventConsumer>();
             // Dispatcher
             services.AddEventuateTramDomainEventDispatcher(Guid.NewGuid().ToString(),
             provider => DomainEventHandlersBuilder.ForAggregateType(typeof(Customer).Name)
              .OnEvent<CustomerCreatedEvent, TestCustomerEventConsumer>()
              .OnEvent<CustomerValidationFailedEvent, TestCustomerEventConsumer>()
              .OnEvent<CustomerCreditReservedEvent, TestCustomerEventConsumer>()
              .OnEvent<CustomerCreditReservationFailedEvent, TestCustomerEventConsumer>()
              .Build());
             // Repository
             services.AddTransient<ICustomerRepository, CustomerRepository>();
         }).Build();
            host.StartAsync().Wait();

            //Services
            domainEventPublisher = host.Services.GetService<IDomainEventPublisher>();
            consumer = host.Services.GetService<TestCustomerEventConsumer>();
            var customerRepository = host.Services.GetService<ICustomerRepository>();
            customerDataService = new CustomerDataService(customerRepository, domainEventPublisher);
            //Initialize Money
            creditLimit = new Money("12.10");
        }
        [TestMethod]
        public void CustomerShouldbeCreatedAndEventShouldbePublished()
        {
            //Create Customer
            var customer = customerDataService.CreateCustomer("Joe", creditLimit);
            // assert
            Assert.IsNotNull(customer.Id);
            CustomerCreatedEvent customerCreatedEvent;
            consumer.GetCustomerCreatedEventQueue().TryTake(out customerCreatedEvent, TimeSpan.FromSeconds(20));

            Assert.IsNotNull(customerCreatedEvent);
            Assert.AreEqual("Joe", customerCreatedEvent.Name);
        }

        [TestMethod]
        public void ReserveCreditAndEventShouldBePublished()
        {
            //Create Customer
            var customer = customerDataService.CreateCustomer("Joe", creditLimit);
            // assert
            Assert.IsNotNull(customer.Id);
            CustomerCreatedEvent customerCreatedEvent;
            consumer.GetCustomerCreatedEventQueue().TryTake(out customerCreatedEvent, TimeSpan.FromSeconds(20));
            //assert
            Assert.IsNotNull(customerCreatedEvent);
            Assert.AreEqual("Joe", customerCreatedEvent.Name);

            //Reserve Credit
            Money orderTotal = new Money("5.10");
            long orderId = System.DateTime.Now.Ticks;
            customerDataService.ReserveCredit(orderId, customer.Id, orderTotal);
            CustomerCreditReservedEvent customerCreditReservedEvent;
            consumer.GetCustomerCreditReservedEventQueue().TryTake(out customerCreditReservedEvent, TimeSpan.FromSeconds(20));
            //assert
            Assert.IsNotNull(customerCreditReservedEvent);
        }
        [TestMethod]
        public void CreditLimitExceededExceptionEventShouldBePublished()
        {
            //Create Customer
            var customer = customerDataService.CreateCustomer("Joe", creditLimit);
            //assert
            Assert.IsNotNull(customer.Id);
            CustomerCreatedEvent customerCreatedEvent;
            consumer.GetCustomerCreatedEventQueue().TryTake(out customerCreatedEvent, TimeSpan.FromSeconds(20));
            //assert
            Assert.IsNotNull(customerCreatedEvent);
            Assert.AreEqual("Joe", customerCreatedEvent.Name);

            //Reserve Credit with OrderTotal more than Customer's CreditLimit 
            Money orderTotal = new Money("20.10");
            long orderId = System.DateTime.Now.Ticks;
            customerDataService.ReserveCredit(orderId, customer.Id, orderTotal);
            CustomerCreditReservationFailedEvent customerCreditReservationFailedEvent;
            consumer.GetCustomerCreditReservationFailedEventQueue().TryTake(out customerCreditReservationFailedEvent, TimeSpan.FromSeconds(20));
            //assert
            Assert.IsNotNull(customerCreditReservationFailedEvent);
        }

        [TestMethod]
        public void CustomerValidationFailedEventShouldBePublished()
        {
            //Reserve Credit with random CustomerId
            Money orderTotal = new Money("20.10");
            long orderId = System.DateTime.Now.Ticks;
            long customerId = System.DateTime.Now.Ticks;
            customerDataService.ReserveCredit(orderId, customerId, orderTotal);
            CustomerValidationFailedEvent customerValidationFailedEvent;
            consumer.GetCustomerValidationFailedEventQueue().TryTake(out customerValidationFailedEvent, TimeSpan.FromSeconds(20));
            //assert
            Assert.IsNotNull(customerValidationFailedEvent);
        }

        [TestMethod]
        public void ReleaseCreditAndEventShouldBePublished()
        {
            //Create Customer
            var customer = customerDataService.CreateCustomer("Joe", creditLimit);
            //assert
            Assert.IsNotNull(customer.Id);
            CustomerCreatedEvent customerCreatedEvent;
            consumer.GetCustomerCreatedEventQueue().TryTake(out customerCreatedEvent, TimeSpan.FromSeconds(20));
            //assert
            Assert.IsNotNull(customerCreatedEvent);
            Assert.AreEqual("Joe", customerCreatedEvent.Name);

            //Reserve Credit
            Money orderTotal = new Money("10.10");
            long orderId = System.DateTime.Now.Ticks;
            customerDataService.ReserveCredit(orderId, customer.Id, orderTotal);
            CustomerCreditReservedEvent customerCreditReservedEvent;
            consumer.GetCustomerCreditReservedEventQueue().TryTake(out customerCreditReservedEvent, TimeSpan.FromSeconds(20));
            //assert
            Assert.IsNotNull(customerCreditReservedEvent);

            //Release Credit
            customerDataService.ReleaseCredit(orderId, customer.Id);

            //Reserve Credit again
            customerDataService.ReserveCredit(System.DateTime.Now.Ticks, customer.Id, orderTotal);
            CustomerCreditReservedEvent customerCreditReservedEventAfterRelease;
            consumer.GetCustomerCreditReservedEventQueue().TryTake(out customerCreditReservedEventAfterRelease, TimeSpan.FromSeconds(20));
            //assert
            Assert.IsNotNull(customerCreditReservedEventAfterRelease);

        }
    }
}
