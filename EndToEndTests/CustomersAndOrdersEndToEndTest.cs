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
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Threading;

namespace EndToEndTests
{
    [TestClass]
    public class CustomersAndOrdersEndToEndTest
    {
        string hostCustmer = "http://localhost:8081/";
        string hostOrder = "http://localhost:8082/";
        public CustomersAndOrdersEndToEndTest()
        {
        }
        [TestMethod]
        public async Task CustomerShouldbeCreated()
        {
            using (var client = new HttpClient())
            {
                var uri = hostCustmer + "customer";
                CreateCustomerRequest request = new CreateCustomerRequest();
                request.Name = "Joe";
                request.CreditLimit = new Money("12.30");
                var serializeObj = JsonSerializer.Serialize(request);
                var response = await RestAPICall(uri, serializeObj);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                string jsonString = await response.Content.ReadAsStringAsync();
                var customerResponse = JsonSerializer.Deserialize<CreateCustomerResponse>(jsonString, SerializerOptions());
                Assert.IsNotNull(customerResponse.CustomerId);
            }
        }
        [TestMethod]
        public async Task OrderShouldbeCreated()
        {
            using (var client = new HttpClient())
            {
                var uri = hostOrder + "order";
                CreateOrderRequest request = new CreateOrderRequest();
                request.CustomerId = 1;
                request.OrderTotal = new Money("12.30");
                var serializeObj = JsonSerializer.Serialize(request);
                var response = await RestAPICall(uri, serializeObj);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                string jsonString = await response.Content.ReadAsStringAsync();
                var orderResponse = JsonSerializer.Deserialize<CreateOrderResponse>(jsonString, SerializerOptions());
                Assert.IsNotNull(orderResponse.OrderId);
            }
        }
        [TestMethod]
        public async Task ShouldApprove()
        {
            var customerId = await CreateCustomer("Joe", "50.30");
            Assert.IsNotNull(customerId);
            var orderId = await CreateOrder(customerId, "20.30");
            Assert.IsNotNull(orderId);
            await AssertOrderState(orderId, OrderState.APPROVED);
        }
        [TestMethod]
        public async Task ShouldReject()
        {
            var customerId = await CreateCustomer("Joe", "50.30");
            Assert.IsNotNull(customerId);
            var orderId = await CreateOrder(customerId, "120.50");
            Assert.IsNotNull(orderId);
            await AssertOrderState(orderId, OrderState.REJECTED);
        }
        private async Task<long> CreateCustomer(string name, string amount)
        {
            var uri = hostCustmer + "customer";
            CreateCustomerRequest request = new CreateCustomerRequest();
            request.Name = name;
            request.CreditLimit = new Money(amount);
            var serializeObj = JsonSerializer.Serialize(request);
            var response = await RestAPICall(uri, serializeObj);
            string jsonString = await response.Content.ReadAsStringAsync();
            var customerResponse = JsonSerializer.Deserialize<CreateCustomerResponse>(jsonString, SerializerOptions());
            return customerResponse.CustomerId;
        }
        private async Task<long> CreateOrder(long customerId, string amount)
        {
            var uri = hostOrder + "order";
            CreateOrderRequest request = new CreateOrderRequest();
            request.CustomerId = customerId;
            request.OrderTotal = new Money(amount);
            var serializeObj = JsonSerializer.Serialize(request);
            var response = await RestAPICall(uri, serializeObj);
            string jsonString = await response.Content.ReadAsStringAsync();
            var orderResponse = JsonSerializer.Deserialize<CreateOrderResponse>(jsonString, SerializerOptions());
            return orderResponse.OrderId;
        }
        private async Task AssertOrderState(long orderId, OrderState orderState)
        {
            Thread.Sleep(30000); // Wait
            using (var client = new HttpClient())
            {
                var uri = hostOrder + "order/" + orderId;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.GetAsync(uri);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                string jsonString = await response.Content.ReadAsStringAsync();
                var orderResponse = JsonSerializer.Deserialize<GetOrderResponse>(jsonString, SerializerOptions());
                Assert.AreEqual(orderState, orderResponse.State);
            }
        }
        private async Task<HttpResponseMessage> RestAPICall(string uri, string serializeRequest)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.PostAsync(uri, new StringContent(serializeRequest, Encoding.UTF8, "application/json"));
                return response;
            }
        }
        private JsonSerializerOptions SerializerOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            return options;
        }
    }
}
