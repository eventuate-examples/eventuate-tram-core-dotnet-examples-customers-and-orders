using OrderService.DBContext;
using OrderService.Models;
using OrderService.Repository;
using OrderService.Service;
using IO.Eventuate.Tram;
using IO.Eventuate.Tram.Events.Publisher;
using IO.Eventuate.Tram.Local.Kafka.Consumer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceCommon.Classes;
using System;
using IO.Eventuate.Tram.Events.Subscriber;
using IO.Eventuate.Tram.Events.Common;
using Microsoft.Extensions.Logging;
using ServiceCommon.Helpers;

namespace OrderService.UnitTests.Service
{
    [TestClass]
    public class OrderServiceTests
    {
        OrderService.Service.OrderService orderService;
        IDomainEventPublisher domainEventPublisher;
        TestEventConsumer consumer;
        OrderDetails orderDetails;

        [TestInitialize]
        public void Setup()
        {
            var host = new HostBuilder()
         .ConfigureServices((hostContext, services) =>
         {
             //DbContext
             services.AddDbContext<OrderContext>(o => o.UseSqlServer(TestSettings.EventuateDB));
             // Kafka Transport
             services.AddEventuateTramSqlKafkaTransport(TestSettings.EventuateTramDbSchema, TestSettings.KafkaBootstrapServers, EventuateKafkaConsumerConfigurationProperties.Empty(),
                (provider, o) =>
                {
                    var applicationDbContext = provider.GetRequiredService<OrderContext>();
                   // applicationDbContext.Database.Migrate();
                    o.UseSqlServer(applicationDbContext.Database.GetDbConnection());
                });
             // Publisher
             services.AddEventuateTramEventsPublisher();
             // Consumer
             services.AddSingleton<TestEventConsumer>();
             // Dispatcher
             services.AddEventuateTramDomainEventDispatcher(Guid.NewGuid().ToString(),
            provider => DomainEventHandlersBuilder.ForAggregateType(typeof(Order).Name)
                .OnEvent<OrderCreatedEvent, TestEventConsumer>()
                .OnEvent<OrderCancelledEvent, TestEventConsumer>()
                .OnEvent<OrderRejectedEvent, TestEventConsumer>()
                .OnEvent<OrderApprovedEvent, TestEventConsumer>()
                .Build());
             // Repository
             services.AddTransient<IOrderRepository, OrderRepository>();
         }).Build();
            host.StartAsync().Wait();

            //Services
            domainEventPublisher = host.Services.GetService<IDomainEventPublisher>();
            consumer = host.Services.GetService<TestEventConsumer>();
            var orderRepository = host.Services.GetService<IOrderRepository>();
            orderService = new OrderService.Service.OrderService(orderRepository, domainEventPublisher);
            //Initialize Money
            Money orderTotal = new Money("12.10");
            orderDetails = new OrderDetails(1, orderTotal);
        }
        [TestMethod]
        public void OrderShouldbeApprovedAndEventShouldbePublished()
        {
            //Create Order
            var order = orderService.CreateOrder(orderDetails);
            Assert.IsNotNull(order.Id);
            AssertEvent<OrderCreatedEvent>();

            //Approve Order
            orderService.ApproveOrder(order.Id);
            AssertEvent<OrderApprovedEvent>();
        }
        [TestMethod]
        public void OrderShouldbeCancelledAndEventShouldbePublished()
        {
            //Create Order
            var order = orderService.CreateOrder(orderDetails);
            Assert.IsNotNull(order.Id);
            AssertEvent<OrderCreatedEvent>();
            //Approve Order
            orderService.ApproveOrder(order.Id);
            AssertEvent<OrderApprovedEvent>();
            //Cancel Order
            orderService.CancelOrder(order.Id);
            AssertEvent<OrderCancelledEvent>();
        }
        [TestMethod]
        public void OrderShouldbeRejectedAndEventShouldbePublished()
        {
            //Create Order
            var order = orderService.CreateOrder(orderDetails);
            Assert.IsNotNull(order.Id);
            AssertEvent<OrderCreatedEvent>();
            //Reject Order
            orderService.RejectOrder(order.Id);
            AssertEvent<OrderRejectedEvent>();
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
                        T orderEvent = (T)domainEvent;
                        //Assert
                        Assert.IsNotNull(orderEvent);
                        Assert.AreEqual(orderDetails.OrderTotal.Amount, ((OrderDetails)orderEvent.GetType().GetProperty("OrderDetails").GetValue(orderEvent)).OrderTotal.Amount);
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
