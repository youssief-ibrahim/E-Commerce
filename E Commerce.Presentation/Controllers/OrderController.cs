using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Services_Abstraction;
using E_Commerce.Shared.DTOS.OrderDTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Presentation.Controllers
{
    public class OrderController : ApiBaseController
    {
        private readonly IOrderService orderService;
        public OrderController(IOrderService _orderService)
        {
            orderService = _orderService;
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<OrderToReturnDto>> CreateOrder(OrderDto orderDto)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await orderService.CreateOrderAsync(orderDto, UserId!);
            return HandleResult(order);
        }
        [HttpGet("DelivaryMethod")]
        public async Task<ActionResult<IEnumerable<DeliveryMethodDto>>> GetAllDeliveryMethod()
        {
            var deliveryMethods = await orderService.GetAllDeliveryMethodAsync();
            return Ok(deliveryMethods);
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetAllOrderByUserId(string email)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await orderService.GetAllOrdersAsync(UserId!);
            return Ok(order);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdAndUserId(Guid id)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await orderService.GetOrderByIdAndUserIdAsync(id, UserId!);
            return Ok(order);
        }
        // Cancel Order
        [HttpPost("{id}/cancel")]
        [Authorize]
        public async Task<ActionResult<OrderToReturnDto>> CancelOrder(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await orderService.CancelOrderAsync(id, userId!);

            return HandleResult(result);
        }
        // Update Order Status - Admin Only
        [HttpPost("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<OrderToReturnDto>> UpdateOrderStatus( Guid id, OrderStatusDto status)
        {
            var result = await orderService.UpdateOrderStatusAsync(id, status);

            return HandleResult(result);
        }

    }
}
