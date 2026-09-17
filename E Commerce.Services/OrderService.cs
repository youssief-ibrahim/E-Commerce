using AutoMapper;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.OrdersModule;
using E_Commerce.Domain.Entities.ProductModule;
using E_Commerce.Services.Specifications;
using E_Commerce.Services_Abstraction;
using E_Commerce.Shared.CommonResult;
using E_Commerce.Shared.DTOS.OrderDTOS;
using Microsoft.Extensions.Configuration;
using Stripe;
using Product = E_Commerce.Domain.Entities.ProductModule.Product;

namespace E_Commerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper mapper;
        private readonly IBasketRepository basketRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration config;

        public OrderService(IMapper _mapper, IBasketRepository _basketRepository, IUnitOfWork _unitOfWork, IConfiguration _config)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            basketRepository = _basketRepository;
            config = _config;
        }

        public async Task<Result<OrderToReturnDto>> CancelOrderAsync(Guid id, string userId)
        {
            var specification = new OrderSpecification(id, userId);

            var order = await unitOfWork.GetRepository<Order, Guid>().GetByIdWithSpecificationAsync(specification);

            if (order == null) return Error.NotFound("Order Not Found", $"Order with id {id} was not found");

            if (order.OrderStatus != OrderStatus.Pending &&
                order.OrderStatus != OrderStatus.PaymentReceived)
                return Error.Failure("Order Cannot Be Cancelled", "This order cannot be cancelled at its current status");

            await RestoreStockAsync(order);

            order.OrderStatus = OrderStatus.Cancelled;

            unitOfWork.GetRepository<Order, Guid>().Update(order);

            var result = await unitOfWork.SaveChangeAsync();

            if (result == 0)
                return Error.Failure("Cancel Failed", "An error occurred while cancelling the order");

            return mapper.Map<OrderToReturnDto>(order);
        }

        public async Task<Result<OrderToReturnDto>> CreateOrderAsync(OrderDto orderDto, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Error.Unauthorized("Unauthorized", "User is not authenticated");

            var adress = mapper.Map<OrderAddress>(orderDto.Address);

            var diliveryMethod = await unitOfWork.GetRepository<DeliveryMethod, int>().GetByIdAsync(orderDto.DeliveryMethodId);
            if (diliveryMethod == null) return Error.NotFound("Delivery Method Not Found", $"Delivery Method with {orderDto.DeliveryMethodId} is Not Found");

            var basket = await basketRepository.GetBasketAsync(orderDto.BasketId);
            if (basket == null) return Error.NotFound("Basket not found", $"Basket with id {orderDto.BasketId} not found");

            if (basket.Items == null || !basket.Items.Any())
                return Error.Validation("Basket is empty", "Cannot create an order from an empty basket");

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
                return Error.Validation("Payment Intent Id is null", "Cannot create order without a valid payment intent id");

            var spec = new OrderwithPaymentIntentSpecefication(basket.PaymentIntentId);
            var existingOrder = await unitOfWork.GetRepository<Order, Guid>().GetByIdWithSpecificationAsync(spec);
            if (existingOrder is not null)
            {
                if (existingOrder.OrderStatus is OrderStatus.PaymentReceived or OrderStatus.Shipped or OrderStatus.Delivered)
                    return Error.Validation("Order already exists", "An order for this payment has already been completed");

                if (existingOrder.OrderStatus is OrderStatus.Pending)
                    await RestoreStockAsync(existingOrder);

                unitOfWork.GetRepository<Order, Guid>().Delete(existingOrder);
                var Resultsave = await unitOfWork.SaveChangeAsync();
                if (Resultsave == 0) return Error.Failure("Failed to delete existing order", "An error occurred while deleting the existing order");
            }

            var productRepo = unitOfWork.GetRepository<Product, int>();
            List<OrderItem> orderItems = new List<OrderItem>();

            foreach (var item in basket.Items)
            {
                if (item.Quantity <= 0)
                    return Error.Validation("Invalid quantity", $"Invalid quantity for product id {item.Id}");

                var product = await productRepo.GetByIdAsync(item.Id);
                if (product == null) return Error.NotFound("Product not found", $"Product with id {item.Id} not found");

                if (product.Quantity < item.Quantity)
                    return Error.Validation("Insufficient stock", $"Product '{product.Name}' has only {product.Quantity} item(s) in stock");

                product.Quantity -= item.Quantity;
                productRepo.Update(product);

                var orderItem = new OrderItem()
                {
                    product = new ProductItemOrdered
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
            var orderStatus = await GetStatusFromPaymentIntentAsync(basket.PaymentIntentId);

            Order order = new Order()
            {
                UserId = userId,
                DeliveryMethod = diliveryMethod,
                Address = adress,
                Items = orderItems,
                Subtotal = subTotal,
                PaymentIntentId = basket.PaymentIntentId,
                OrderStatus = orderStatus,
                OrderDate = DateTime.Now
            };

            await unitOfWork.GetRepository<Order, Guid>().AddAsync(order);
            var result = await unitOfWork.SaveChangeAsync();
            if (result == 0) return Error.Failure("Failed to create order", "An error occurred while creating the order");

            await basketRepository.DeleteBasketAsync(orderDto.BasketId);

            return mapper.Map<OrderToReturnDto>(order);
        }

        public async Task<IReadOnlyList<DeliveryMethodDto>> GetAllDeliveryMethodAsync()
        {
            var delivary = await unitOfWork.GetRepository<DeliveryMethod, int>().GetAllAsync();
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
            var spacifc = new OrderSpecification(id, userId);
            var order = await unitOfWork.GetRepository<Order, Guid>().GetByIdWithSpecificationAsync(spacifc);
            if (order == null)
                return Error.NotFound("Order Not Found", $"Order with id {id} was not found");

            return mapper.Map<OrderToReturnDto>(order);
        }

        public async Task<Result<OrderToReturnDto>> UpdateOrderStatusAsync(Guid id, OrderStatusDto statusdTO)
        {
            var order = await unitOfWork.GetRepository<Order, Guid>().GetByIdWithSpecificationAsync(new OrderSpecification(id));

            if (order == null)
                return Error.NotFound("Order Not Found", $"Order with id {id} was not found");

            var newStatus = mapper.Map<OrderStatus>(statusdTO);

            var currentStatus = order.OrderStatus;

            if (currentStatus == newStatus)
                return Error.Failure("Invalid Status", "The order already has this status");

            if (!IsValidStatusTransition(currentStatus, newStatus))
                return Error.Failure("Invalid Status Transition", $"Cannot change order status from {currentStatus} to {newStatus}");

            if (newStatus == OrderStatus.Cancelled &&
                (currentStatus == OrderStatus.Pending || currentStatus == OrderStatus.PaymentReceived))
            {
                await RestoreStockAsync(order);
            }

            order.OrderStatus = newStatus;

            unitOfWork.GetRepository<Order, Guid>().Update(order);

            var result = await unitOfWork.SaveChangeAsync();

            if (result == 0)
                return Error.Failure("Update Failed", "An error occurred while updating the order status");

            return mapper.Map<OrderToReturnDto>(order);
        }

        public async Task<Result> UpdatePaymentStatusByIntentIdAsync(string paymentIntentId, bool isSucceeded)
        {
            var order = await unitOfWork.GetRepository<Order, Guid>()
                .GetByIdWithSpecificationAsync(new OrderwithPaymentIntentSpecefication(paymentIntentId));

            if (order == null)
                return Error.NotFound("Order not found", $"Order not found for PaymentIntent ID: {paymentIntentId}");

            if (isSucceeded)
            {
                if (order.OrderStatus is OrderStatus.Cancelled or OrderStatus.PaymentFailed)
                    return Error.Validation("Invalid order status", "Cannot mark a cancelled or failed order as paid");

                if (order.OrderStatus == OrderStatus.PaymentReceived)
                    return Result.Ok();

                order.OrderStatus = OrderStatus.PaymentReceived;
            }
            else
            {
                if (order.OrderStatus is OrderStatus.Shipped or OrderStatus.Delivered)
                    return Error.Validation("Invalid order status", "Cannot mark a shipped or delivered order as failed");

                if (order.OrderStatus == OrderStatus.PaymentFailed)
                    return Result.Ok();

                if (order.OrderStatus is OrderStatus.Pending or OrderStatus.PaymentReceived)
                    await RestoreStockAsync(order);

                order.OrderStatus = OrderStatus.PaymentFailed;
            }

            unitOfWork.GetRepository<Order, Guid>().Update(order);
            await unitOfWork.SaveChangeAsync();
            return Result.Ok();
        }

        private async Task RestoreStockAsync(Order order)
        {
            if (order.Items is null || order.Items.Count == 0)
                return;

            var productRepo = unitOfWork.GetRepository<Product, int>();
            foreach (var item in order.Items)
            {
                var product = await productRepo.GetByIdAsync(item.product.ProductId);
                if (product is null) continue;

                product.Quantity += item.Quantity;
                productRepo.Update(product);
            }
        }

        private async Task<OrderStatus> GetStatusFromPaymentIntentAsync(string paymentIntentId)
        {
            try
            {
                StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
                var paymentIntent = await new PaymentIntentService().GetAsync(paymentIntentId);
                return paymentIntent.Status == "succeeded"
                    ? OrderStatus.PaymentReceived
                    : OrderStatus.Pending;
            }
            catch
            {
                return OrderStatus.Pending;
            }
        }

        private static bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            return currentStatus switch
            {
                OrderStatus.Pending =>
                 newStatus == OrderStatus.PaymentReceived ||
                 newStatus == OrderStatus.Cancelled ||
                 newStatus == OrderStatus.PaymentFailed,

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
