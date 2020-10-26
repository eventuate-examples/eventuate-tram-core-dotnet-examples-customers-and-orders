﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IO.Eventuate.Tram.Events.Publisher;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        OrderDataService orderService;
        public OrderController(OrderDataService _orderService)
        {
            orderService = _orderService;
        }
        [HttpPost]
        public IActionResult CreateOrder([FromBody] CreateOrderRequest request)
        {
            OrderDetails orderDetails = new OrderDetails(request.CustomerId, request.OrderTotal);
            Order order = orderService.CreateOrder(orderDetails);
            CreateOrderResponse createOrderResponse = new CreateOrderResponse(order.Id);
            return Ok(createOrderResponse);
        }
        [HttpGet]
        [Route("{id:long}")]
        public IActionResult GetOrder([FromRoute] long id)
        {
            Order order = orderService.GetOrder(id);
            GetOrderResponse getOrderResponse = new GetOrderResponse(order.Id, order.OrderDetails, order.State, order.Version);
            return Ok(getOrderResponse);
        }
    }
}