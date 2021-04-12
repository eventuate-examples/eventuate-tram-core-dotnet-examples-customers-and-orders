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
using ServiceCommon.OrderHistoryCommon;
using ServiceCommon.OrderHistoryTextSearchCommon;
using System.Collections.Generic;

namespace EndToEndTests
{
    [TestClass]
    public class CustomersAndOrdersEndToEndTest
    {
        string urlCustomer = "http://localhost:8081/customer";
        string urlOrder = "http://localhost:8082/order";
        string urlOrderHistory = "http://localhost:8083/customers";
        string urlOrderHistoryTextSearch = "http://localhost:8084";
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
        [TestMethod]
        public void ShouldRejectForNonExistentCustomerId()
        {
            long customerId = System.DateTime.Today.Ticks;
            long orderId = CreateOrder(customerId, "120.50");
            AssertOrderState(orderId, OrderState.REJECTED);
        }
        [TestMethod]
        public void ShouldCancel()
        {
            long customerId = CreateCustomer("Joe", "50.30");
            long orderId = CreateOrder(customerId, "20.50");
            AssertOrderState(orderId, OrderState.APPROVED);
            CancelOrder(orderId);
            AssertOrderState(orderId, OrderState.CANCELLED);
        }
        [TestMethod]
        public void ShouldRejectApproveAndKeepOrdersInHistory()
        {
            long customerId = CreateCustomer("Joe", "1000");
            long order1Id = CreateOrder(customerId, "100");
            AssertOrderState(order1Id, OrderState.APPROVED);
            long order2Id = CreateOrder(customerId, "1000");
            AssertOrderState(order2Id, OrderState.REJECTED);
            Util.Eventually(100, 1000, () =>
            {
                CustomerView customerView = GetCustomerView(customerId);
                var orders = customerView.Orders;
                Assert.AreEqual(2, orders.Count);
                Assert.AreEqual(orders[order1Id].State, OrderState.APPROVED);
                Assert.AreEqual(orders[order2Id].State, OrderState.REJECTED);
            });
        }
        [TestMethod]
        public void CustomerShouldReturnInSearch()
        {
            long customerId = CreateCustomer("John", "500");
            Util.Eventually(50, 3000, () =>
            {
                List<CustomerTextView> customerTextView = GetCustomerTextView("John");
                Assert.IsTrue(customerTextView.Count > 0);
                //Assert.AreEqual(customerId.ToString(), customerTextView[0].id);
            });
        }
        [TestMethod]
        public void OrderShouldReturnInSearch()
        {
            long customerId = CreateCustomer("Bob", "200");
            var orderId = CreateOrder(customerId, "15.56");
            Util.Eventually(50, 3000, () =>
            {
                List<OrderTextView> orderTextView = GetOrderTextView("15.56");
                Assert.IsTrue(orderTextView.Count > 0);
                //Assert.AreEqual(orderId.ToString(), orderTextView[0].id);
            });
        }
        private List<CustomerTextView> GetCustomerTextView(string customerId)
        {
            var customerTextView = WebApiHelper.WebApiTextSearch<CustomerTextView>("GET", urlOrderHistoryTextSearch + "/customers?search=" + customerId);
            Assert.IsNotNull(customerTextView);
            return customerTextView;
        }
        private List<OrderTextView> GetOrderTextView(string orderId)
        {
            var orderTextView = WebApiHelper.WebApiTextSearch<OrderTextView>("GET", urlOrderHistoryTextSearch + "/orders?search=" + orderId);
            Assert.IsNotNull(orderTextView);
            return orderTextView;
        }

        private CustomerView GetCustomerView(long customerId)
        {
            var customerView = WebApiHelper.WebApiCall<CustomerView>("GET", urlOrderHistory + "/" + customerId, null);
            Assert.IsNotNull(customerView);
            return customerView;
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
        private void CancelOrder(long orderId)
        {
            var orderResponse = WebApiHelper.WebApiCall<GetOrderResponse>("POST", urlOrder + "/" + orderId + "/cancel", null);
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
