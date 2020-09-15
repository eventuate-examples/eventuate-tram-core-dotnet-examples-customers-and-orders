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
using ServiceCommon.Common;

namespace CustomerService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        ICustomerRepository customerRepository;
        IDomainEventPublisher domainEventPublisher;
        public CustomerController(ICustomerRepository _customerRepository, IDomainEventPublisher _domainEventPublisher)
        {
            customerRepository = _customerRepository;
            domainEventPublisher = _domainEventPublisher;
        }
        [HttpPost]
        public IActionResult CreateCustomer([FromBody] CreateCustomerRequest request)
        {
            CustomerDataService customerService = new CustomerDataService(customerRepository, domainEventPublisher);
            Customer customer = customerService.CreateCustomer(request.Name, request.CreditLimit);
            CreateCustomerResponse createCustomerResponse = new CreateCustomerResponse(customer.Id);
            return Ok(createCustomerResponse);
        }
    }
}
