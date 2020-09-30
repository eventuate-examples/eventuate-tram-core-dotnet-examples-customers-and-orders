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

namespace EndToEndTests
{
    [TestClass]
    public class CustomersAndOrdersEndToEndTest
    {
        long uniqueId = System.DateTime.Now.Ticks;
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
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.PostAsync(uri, new StringContent(serializeObj, Encoding.UTF8, "application/json"));
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                string jsonString = await response.Content.ReadAsStringAsync();
                var customerResponse = JsonSerializer.Deserialize<CreateCustomerResponse>(jsonString);
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
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.PostAsync(uri, new StringContent(serializeObj, Encoding.UTF8, "application/json"));
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                string jsonString = await response.Content.ReadAsStringAsync();
                var orderResponse = JsonSerializer.Deserialize<CreateOrderResponse>(jsonString);
				Assert.IsNotNull(orderResponse.OrderId);
            }
        }
    }
}
