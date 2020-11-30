using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerService.Models;
using CustomerService.Repository;
using CustomerService.Service;
using IO.Eventuate.Tram.Events.Publisher;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using ServiceCommon.Classes;
using ServiceCommon.Common;

namespace CustomerService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        CustomerDataService customerService;
        public CustomerController(CustomerDataService _customerService)
        {
            customerService = _customerService;
        }
        [HttpPost]
        public IActionResult CreateCustomer([FromBody] CreateCustomerRequest request)
        {
            Customer customer = customerService.CreateCustomer(request.Name, request.CreditLimit);
            CreateCustomerResponse createCustomerResponse = new CreateCustomerResponse(customer.Id);
            return Ok(createCustomerResponse);
        }
    }
}
