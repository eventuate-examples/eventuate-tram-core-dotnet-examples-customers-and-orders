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

namespace OrderService.UnitTests.Service
{
    [TestClass]
    public class OrderServiceTests
    {
        OrderDataService orderDataService;
        IDomainEventPublisher domainEventPublisher;
        TestOrderEventConsumer consumer;
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
                    applicationDbContext.Database.Migrate();
                    o.UseSqlServer(applicationDbContext.Database.GetDbConnection());
                });
             // Publisher
             services.AddEventuateTramEventsPublisher();
             // Consumer
             services.AddSingleton<TestOrderEventConsumer>();
             // Dispatcher
             services.AddEventuateTramDomainEventDispatcher(Guid.NewGuid().ToString(),
            provider => DomainEventHandlersBuilder.ForAggregateType(typeof(Order).Name)
                .OnEvent<OrderCreatedEvent, TestOrderEventConsumer>()
                .OnEvent<OrderCancelledEvent, TestOrderEventConsumer>()
                .OnEvent<OrderRejectedEvent, TestOrderEventConsumer>()
                .OnEvent<OrderApprovedEvent, TestOrderEventConsumer>()
                .Build());
             // Repository
             services.AddTransient<IOrderRepository, OrderRepository>();
         }).Build();
            host.StartAsync().Wait();

            //Services
            domainEventPublisher = host.Services.GetService<IDomainEventPublisher>();
            consumer = host.Services.GetService<TestOrderEventConsumer>();
            var orderRepository = host.Services.GetService<IOrderRepository>();
            orderDataService = new OrderDataService(orderRepository, domainEventPublisher);
            //Initialize Money
            Money orderTotal = new Money("12.10");
            orderDetails = new OrderDetails(1, orderTotal);
        }
        [TestMethod]
        public void OrderShouldbeCreated()
        {
            //Create Order
            var order = orderDataService.CreateOrder(orderDetails);
            //assert
            Assert.IsNotNull(order.Id);
            OrderCreatedEvent orderCreatedEvent;
            consumer.GetOrderCreatedEventQueue().TryTake(out orderCreatedEvent, TimeSpan.FromSeconds(20));
            //assert
            Assert.IsNotNull(orderCreatedEvent);
            Assert.AreEqual(orderDetails.OrderTotal.Amount, orderCreatedEvent.OrderDetails.OrderTotal.Amount);
        }
        [TestMethod]
        public void OrderShouldbeApprovedAndEventShouldbePublished()
        {
            //Create Order
            var order = orderDataService.CreateOrder(orderDetails);
            //assert
            Assert.IsNotNull(order.Id);
            OrderCreatedEvent orderCreatedEvent;
            consumer.GetOrderCreatedEventQueue().TryTake(out orderCreatedEvent, TimeSpan.FromSeconds(20));
            //assert
            Assert.IsNotNull(orderCreatedEvent);

            //Approve Order
            orderDataService.ApproveOrder(order.Id);
            OrderApprovedEvent orderApprovedEvent;
            consumer.GetOrderApprovedEventQueue().TryTake(out orderApprovedEvent, TimeSpan.FromSeconds(20));
            // assert
            Assert.IsNotNull(orderApprovedEvent);
        }
        [TestMethod]
        public void OrderShouldbeCancelledAndEventShouldbePublished()
        {
            //Create order
            var order = orderDataService.CreateOrder(orderDetails);
            //assert
            Assert.IsNotNull(order.Id);
            OrderCreatedEvent orderCreatedEvent;
            consumer.GetOrderCreatedEventQueue().TryTake(out orderCreatedEvent, TimeSpan.FromSeconds(20));
            //assert
            Assert.IsNotNull(orderCreatedEvent);

            //Order should be approved before cancellation
            orderDataService.ApproveOrder(order.Id);

            //Cancel Order
            orderDataService.CancelOrder(order.Id);
            OrderCancelledEvent orderCancelledEvent;
            consumer.GetOrderCancelledEventQueue().TryTake(out orderCancelledEvent, TimeSpan.FromSeconds(20));
            //assert
            Assert.IsNotNull(orderCancelledEvent);
        }
        [TestMethod]
        public void OrderShouldbeRejectedAndEventShouldbePublished()
        {
            //Create Order
            var order = orderDataService.CreateOrder(orderDetails);
            //assert
            Assert.IsNotNull(order.Id);
            OrderCreatedEvent orderCreatedEvent;
            consumer.GetOrderCreatedEventQueue().TryTake(out orderCreatedEvent, TimeSpan.FromSeconds(20));
            //assert
            Assert.IsNotNull(orderCreatedEvent);

            //Reject Order
            orderDataService.RejectOrder(order.Id);
            OrderRejectedEvent orderRejectedEvent;
            consumer.GetOrderRejectedEventQueue().TryTake(out orderRejectedEvent, TimeSpan.FromSeconds(20));
            //assert
            Assert.IsNotNull(orderRejectedEvent);
        }
    }
}
