using OrderService.DBContext;
using OrderService.Models;
using OrderService.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceCommon.Classes;

namespace OrderService.UnitTests.Repository
{
    [TestClass]
    public class OrderRepositoryTests
    {
        OrderRepository orderRepository;
        Order order;

        [TestInitialize]
        public void Setup()
        {
            //SetUp OrderContext with InMemoryDatabase
            var options = new DbContextOptionsBuilder<OrderContext>().UseInMemoryDatabase(databaseName: "TestDatabase").Options;
            var context = new OrderContext(options);
            orderRepository = new OrderRepository(context);
            //Initialize
            Money orderTotal = new Money("12.10");
            OrderDetails orderDetails = new OrderDetails(1, orderTotal);
            order = new Order(orderDetails);
        }
        [TestMethod]
        public void OrderShouldbeCreated()
        {
            var response = orderRepository.Add(order);
            // assert
            Assert.IsNotNull(response.Id);
        }
        [TestMethod]
        public void OrderShouldbeFetched()
        {
            var response = orderRepository.Add(order);
            // assert
            Assert.IsNotNull(response.Id);
            var fetchedOrder = orderRepository.FindById(response.Id);
            // assert
            Assert.IsNotNull(fetchedOrder);
        }
        [TestMethod]
        public void OrderShouldbeUpdated()
        {
            var response = orderRepository.Add(order);
            // assert
            Assert.IsNotNull(response.Id);
            var fetchedOrder = orderRepository.FindById(response.Id);
            // assert
            Assert.IsNotNull(fetchedOrder);
            fetchedOrder.State = OrderState.APPROVED;
            orderRepository.Update(fetchedOrder);
            var updatedOrder = orderRepository.FindById(response.Id);
            // assert
            Assert.AreEqual(OrderState.APPROVED, updatedOrder.State);

        }
    }
}