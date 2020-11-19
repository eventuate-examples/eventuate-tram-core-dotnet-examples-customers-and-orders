using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using OrderHistoryService.DBContext;
using OrderHistoryService.Repository;
using ServiceCommon.Classes;
using ServiceCommon.OrderHistoryCommon;

namespace OrderHistoryService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerOrderHistoryController : ControllerBase
    {
        private readonly ICustomerViewRepository _repo;
        public CustomerOrderHistoryController(ICustomerViewRepository repo)
        {
            _repo = repo;
        }
        [HttpGet]
        [Route("/customers/{id:long}")]
        public IActionResult GetCustomer([FromRoute] long id)
        {
            var customer = _repo.FindById(id);
            if (customer == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(customer);
            }
        }
        [HttpGet]
        [Route("/customers/count")]
        public long getCustomerCount()
        {
            return _repo.Count();
        }
    }
}
