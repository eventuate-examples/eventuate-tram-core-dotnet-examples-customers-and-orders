using CustomerService.DBContext;
using CustomerService.Models;
using CustomerService.Repository;
using CustomerService.Service;
using IO.Eventuate.Tram;
using IO.Eventuate.Tram.Events.Publisher;
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
                    applicationDbContext.Database.Migrate();
                    o.UseSqlServer(applicationDbContext.Database.GetDbConnection());
                });
             // Publisher
             services.AddEventuateTramEventsPublisher();
             // Consumer
             services.AddSingleton<TestEventConsumer>();
             // Dispatcher
             services.AddEventuateTramDomainEventDispatcher(Guid.NewGuid().ToString(), provider =>
             {
                 var consumer = provider.GetRequiredService<TestEventConsumer>();
                 return consumer.DomainEventHandlers(typeof(Customer).Name);
             });
             // Repository
             services.AddTransient<ICustomerRepository, CustomerRepository>();
         }).Build();
            host.StartAsync().Wait();

            //Services
            domainEventPublisher = host.Services.GetService<IDomainEventPublisher>();
            consumer = host.Services.GetService<TestEventConsumer>();
            var customerRepository = host.Services.GetService<ICustomerRepository>();
            customerDataService = new CustomerDataService(customerRepository, domainEventPublisher);
            //Initialize Money
            creditLimit = new Money("12.10");
        }
        [TestMethod]
        public void CustomerShouldbeCreatedAndEventShouldbeReceived()
        {
            var customer = customerDataService.CreateCustomer("Joe", creditLimit);
            // assert
            Assert.IsNotNull(customer.Id);

            CustomerCreatedEvent customerCreatedEvent;
            consumer.GetQueue().TryTake(out customerCreatedEvent, TimeSpan.FromSeconds(10));

            Assert.IsNotNull(customerCreatedEvent);
            Assert.AreEqual("Joe", customerCreatedEvent.Name);
        }
    }
}
