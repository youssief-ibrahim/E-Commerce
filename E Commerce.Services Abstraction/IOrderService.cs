using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Shared.CommonResult;
using E_Commerce.Shared.DTOS.OrderDTOS;

namespace E_Commerce.Services_Abstraction
{
    public interface IOrderService
    {
        Task<Result<OrderToReturnDto>> CreateOrderAsync(OrderDto orderDto,string userId);
        Task<IReadOnlyList<DeliveryMethodDto>> GetAllDeliveryMethodAsync();
        Task<IReadOnlyList<OrderToReturnDto>> GetAllOrdersAsync(string userId);
        Task<Result<OrderToReturnDto>> GetOrderByIdAndUserIdAsync(Guid id, string userId);
    }
}
