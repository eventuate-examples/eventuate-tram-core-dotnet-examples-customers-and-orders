using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IO.Eventuate.Tram.Events.Publisher;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Repository;
using OrderService.Service;
using ServiceCommon.Classes;
using ServiceCommon.Common;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        IOrderRepository orderRepository;
        IDomainEventPublisher domainEventPublisher;
        public OrderController(IOrderRepository _orderRepository, IDomainEventPublisher _domainEventPublisher)
        {
            orderRepository = _orderRepository;
            domainEventPublisher = _domainEventPublisher;
        }
        [HttpPost]
        public IActionResult CreateOrder([FromBody] CreateOrderRequest request)
        {
            OrderDataService orderService = new OrderDataService(orderRepository, domainEventPublisher);
            OrderDetails orderDetails = new OrderDetails(request.CustomerId, request.OrderTotal);
            Order order = orderService.CreateOrder(orderDetails);
            CreateOrderResponse createOrderResponse = new CreateOrderResponse(order.Id);
            return Ok(createOrderResponse);
        }
    }
}