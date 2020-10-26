using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ServiceCommon.Common;
using ServiceCommon.Classes;
using System.Threading.Tasks;
using System.Threading;
using ServiceCommon.Helpers;
using System;

namespace EndToEndTests
{
    [TestClass]
    public class CustomersAndOrdersEndToEndTest
    {
        string urlCustomer = "http://localhost:8081/customer";
        string urlOrder = "http://localhost:8082/order";
        public CustomersAndOrdersEndToEndTest()
        {
        }
        [TestMethod]
        public void ShouldApprove()
        {
            var customerId = CreateCustomer("Joe", "50.30");
            Assert.IsNotNull(customerId);
            var orderId = CreateOrder(customerId, "20.30");
            Assert.IsNotNull(orderId);
            AssertOrderState(orderId, OrderState.APPROVED);
        }
        [TestMethod]
        public void ShouldReject()
        {
            var customerId = CreateCustomer("Joe", "50.30");
            Assert.IsNotNull(customerId);
            var orderId = CreateOrder(customerId, "120.50");
            Assert.IsNotNull(orderId);
            AssertOrderState(orderId, OrderState.REJECTED);
        }
        private long CreateCustomer(string name, string amount)
        {
            CreateCustomerRequest request = new CreateCustomerRequest();
            request.Name = name;
            request.CreditLimit = new Money(amount);
            var customerResponse = WebApiHelper.WebApiCall<CreateCustomerResponse>("POST", urlCustomer, JsonSerializer.Serialize(request));
            return customerResponse.CustomerId;
        }
        private long CreateOrder(long customerId, string amount)
        {
            CreateOrderRequest request = new CreateOrderRequest();
            request.CustomerId = customerId;
            request.OrderTotal = new Money(amount);
            var orderResponse = WebApiHelper.WebApiCall<CreateOrderResponse>("POST", urlOrder, JsonSerializer.Serialize(request));
            return orderResponse.OrderId;
        }
        private void AssertOrderState(long orderId, OrderState orderState)
        {
            Util.Eventually(100, 1000, () =>
             {
                 var orderResponse = WebApiHelper.WebApiCall<GetOrderResponse>("GET", urlOrder + "/" + orderId, null);
                 Assert.AreEqual(orderState, orderResponse.State);
             });
        }
    }
}
