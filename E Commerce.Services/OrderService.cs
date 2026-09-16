using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.OrdersModule;
using E_Commerce.Domain.Entities.ProductModule;
using E_Commerce.Services.Specifications;
using E_Commerce.Services_Abstraction;
using E_Commerce.Shared.CommonResult;
using E_Commerce.Shared.DTOS.OrderDTOS;

namespace E_Commerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper mapper;
        private readonly IBasketRepository basketRepository;
        private readonly IUnitOfWork unitOfWork;
        public OrderService(IMapper _mapper, IBasketRepository _basketRepository, IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            basketRepository = _basketRepository;
        }

        public async Task<Result<OrderToReturnDto>> CancelOrderAsync(Guid id, string userId)
        {
            var specification = new OrderSpecification(id, userId);

            var order = await unitOfWork.GetRepository<Order, Guid>().GetByIdWithSpecificationAsync(specification);

            if (order == null) return Error.NotFound( "Order Not Found", $"Order with id {id} was not found");
            
            if (order.OrderStatus != OrderStatus.Pending &&
                order.OrderStatus != OrderStatus.PaymentReceived)
                return Error.Failure("Order Cannot Be Cancelled","This order cannot be cancelled at its current status");
            
            order.OrderStatus = OrderStatus.Cancelled;

            unitOfWork.GetRepository<Order, Guid>().Update(order);

            var result = await unitOfWork.SaveChangeAsync();

            if (result == 0)
              return Error.Failure("Cancel Failed","An error occurred while cancelling the order");
            
            return mapper.Map<OrderToReturnDto>(order);
        }

        public async Task<Result<OrderToReturnDto>> CreateOrderAsync(OrderDto orderDto, string userId)
        {
            var adress = mapper.Map<OrderAddress>(orderDto.Address);

            var diliveryMethod =await unitOfWork.GetRepository<DeliveryMethod,int>().GetByIdAsync(orderDto.DeliveryMethodId);
            if(diliveryMethod == null) return Error.NotFound($"Delivery Method Not Found ", $"Delivery Method with {orderDto.DeliveryMethodId} is Not Found");

            var basket =await basketRepository.GetBasketAsync(orderDto.BasketId);
            if (basket == null) return Error.NotFound("Basket not found", $"Basket with id {orderDto.BasketId} not found");

            List<OrderItem> orderItems = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var product = await unitOfWork.GetRepository<Product, int>().GetByIdAsync(item.Id);
                if (product == null) return Error.NotFound("Product not found", $"Product with id {item.Id} not found");

                var orderItem = new OrderItem()
                {
                    product=new ProductItemOrdered
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        PictureUrl = product.PictureUrl
                    },
                    Price = product.Price,
                    Quantity = item.Quantity
                };
                orderItems.Add(orderItem);
            }

            var subTotal = orderItems.Sum(i => i.Price * i.Quantity);

            Order order = new Order()
            {
               DeliveryMethod = diliveryMethod,
               Address = adress,
               Items = orderItems,
               Subtotal = subTotal,
            };

            await unitOfWork.GetRepository<Order, Guid>().AddAsync(order);
            var result = await unitOfWork.SaveChangeAsync();
            if (result == 0) return Error.Failure("Failed to create order", "An error occurred while creating the order");

            return mapper.Map<OrderToReturnDto>(order);
        }

        public async Task<IReadOnlyList<DeliveryMethodDto>> GetAllDeliveryMethodAsync()
        {
            var delivary=await unitOfWork.GetRepository<DeliveryMethod, int>().GetAllAsync();
            return mapper.Map<IReadOnlyList<DeliveryMethodDto>>(delivary);
        }

        public async Task<IReadOnlyList<OrderToReturnDto>> GetAllOrdersAsync(string userId)
        {
            var spacifc = new OrderSpecification(userId);
            var orders = await unitOfWork.GetRepository<Order, Guid>().GetAllWithSpecificationAsync(spacifc);
            return mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders);
        }

        public async Task<Result<OrderToReturnDto>> GetOrderByIdAndUserIdAsync(Guid id, string userId)
        {
            var spacifc = new OrderSpecification(id,userId);
            var order = await unitOfWork.GetRepository<Order, Guid>().GetByIdWithSpecificationAsync(spacifc);
            return mapper.Map<OrderToReturnDto>(order);
        }

        public async Task<Result<OrderToReturnDto>> UpdateOrderStatusAsync(Guid id, OrderStatusDto statusdTO)
        {
            var order = await unitOfWork.GetRepository<Order, Guid>().GetByIdWithSpecificationAsync(new OrderSpecification(id.ToString()));

            if (order == null)
               return Error.NotFound("Order Not Found",$"Order with id {id} was not found");
           
            var newStatus = mapper.Map<OrderStatus>(statusdTO);

            var currentStatus = order.OrderStatus;

            if (currentStatus == newStatus)
              return Error.Failure("Invalid Status", "The order already has this status");
            
            if (!IsValidStatusTransition(currentStatus, newStatus))
                     return Error.Failure("Invalid Status Transition",$"Cannot change order status from {currentStatus} to {newStatus}");
            
            // 5. Update status
            order.OrderStatus = newStatus;

            unitOfWork.GetRepository<Order, Guid>().Update(order);

            var result = await unitOfWork.SaveChangeAsync();

            if (result == 0)
            return Error.Failure( "Update Failed","An error occurred while updating the order status");
        
            return mapper.Map<OrderToReturnDto>(order);
        }

        private static bool IsValidStatusTransition(OrderStatus currentStatus,OrderStatus newStatus)
        {
            return currentStatus switch
            {
                OrderStatus.Pending =>
                 newStatus == OrderStatus.PaymentReceived ||
                 newStatus == OrderStatus.Cancelled,

                OrderStatus.PaymentReceived =>
                    newStatus == OrderStatus.Shipped ||
                    newStatus == OrderStatus.Cancelled,

                OrderStatus.Shipped =>
                    newStatus == OrderStatus.Delivered,

                _ => false
            };
        }
    }
}
