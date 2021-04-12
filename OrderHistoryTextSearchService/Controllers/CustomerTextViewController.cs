using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;
using OrderHistoryTextSearchService.Service;
using ServiceCommon.OrderHistoryTextSearchCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderHistoryTextSearchService.Controllers
{
    [ApiController]
    [Route("customers")]
    public class CustomerTextViewController : ControllerBase
    {
        public TextViewService<CustomerTextView> customerTextViewService;
        private readonly IElasticClient _elasticClient;
        public CustomerTextViewController(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
            customerTextViewService = new TextViewService<CustomerTextView>(_elasticClient, CustomerTextView.INDEX, CustomerTextView.TYPE);
        }
        [HttpPost]
        public IActionResult CreateCustomerTextView([FromBody] CustomerTextView customerTextView)
        {
            try
            {
                customerTextViewService.Index(customerTextView);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public IActionResult Search([FromQuery] string search)
        {
            var result = customerTextViewService.Search(search);
            return Ok(result);
        }
    }
}
