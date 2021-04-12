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
    [Route("orders")]
    public class OrderTextViewController : ControllerBase
    {
        public TextViewService<OrderTextView> orderTextViewService;
        private readonly IElasticClient _elasticClient;
        public OrderTextViewController(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
            orderTextViewService = new TextViewService<OrderTextView>(_elasticClient, OrderTextView.INDEX, OrderTextView.TYPE);
        }
        [HttpPost]
        public IActionResult CreateOrderTextView([FromBody] OrderTextView orderTextView)
        {
            try
            {
                orderTextViewService.Index(orderTextView);
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
            var result = orderTextViewService.Search(search);
            return Ok(result);
        }
    }
}
