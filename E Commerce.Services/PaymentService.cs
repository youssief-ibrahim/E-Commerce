using AutoMapper;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.OrdersModule;
using E_Commerce.Services_Abstraction;
using E_Commerce.Shared.CommonResult;
using E_Commerce.Shared.DTOS.BasketDTOS;
using Microsoft.Extensions.Configuration;
using Stripe;
using Product = E_Commerce.Domain.Entities.ProductModule.Product;

namespace E_Commerce.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBasketRepository basketRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IConfiguration config;
        private readonly IOrderService orderService;

        public PaymentService(
            IBasketRepository _basketRepository,
            IUnitOfWork _unitOfWork,
            IMapper _mapper,
            IConfiguration _config,
            IOrderService _orderService)
        {
            basketRepository = _basketRepository;
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            config = _config;
            orderService = _orderService;
        }

        public async Task<Result<CartDto>> CreateOrUpdatePaymentIntentAsync(string basketId)
        {
            StripeConfiguration.ApiKey = config["Stripe:SecretKey"];

            var basket = await basketRepository.GetBasketAsync(basketId);
            if (basket == null) return Error.NotFound("Basket Not Found");

            if (basket.Items == null || !basket.Items.Any())
                return Error.Validation("Basket is empty", "Cannot create payment intent for an empty basket.");

            if (basket.DeliveryMethodId == null)
                return Error.Validation("Please Select Delivery Method");

            var deliveryMethod = await unitOfWork
                .GetRepository<DeliveryMethod, int>()
                .GetByIdAsync(basket.DeliveryMethodId.Value);

            if (deliveryMethod == null)
                return Error.NotFound("Delivery Method Not Found");

            foreach (var item in basket.Items)
            {
                if (item.Quantity <= 0)
                    return Error.Validation("Invalid quantity", $"Invalid quantity for product id {item.Id}");

                var product = await unitOfWork.GetRepository<Product, int>().GetByIdAsync(item.Id);
                if (product is null) return Error.NotFound("Product not found", $"Product with id {item.Id} not found");

                if (product.Quantity < item.Quantity)
                    return Error.Validation("Insufficient stock", $"Product '{product.Name}' has only {product.Quantity} item(s) in stock");

                item.ProductName = product.Name;
                item.PictureUrl = product.PictureUrl;
                item.Price = product.Price;
            }

            var shippingPrice = deliveryMethod.Price;
            basket.ShippingPrice = shippingPrice;
            var totalAmount = basket.Items.Sum(i => i.Price * i.Quantity) + shippingPrice;
            var amount = (long)(totalAmount * 100);

            var paymentIntentService = new PaymentIntentService();
            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var Options = new PaymentIntentCreateOptions()
                {
                    Amount = amount,
                    Currency = "usd",
                    PaymentMethodTypes = ["card"],
                };
                var paymentIntent = await paymentIntentService.CreateAsync(Options);
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else
            {
                var Options = new PaymentIntentUpdateOptions()
                {
                    Amount = amount,
                };
                var paymentIntent = await paymentIntentService.UpdateAsync(basket.PaymentIntentId, Options);
                basket.ClientSecret = paymentIntent.ClientSecret;
            }

            await basketRepository.CreateOrUpdateBasketAsync(basket);

            return mapper.Map<CartDto>(basket);
        }

        public async Task<Result> UpdateOrderPaymentSucceededAsync(string request, string stripeSignure)
        {
            var endpointSecret = config["Stripe:EndpointSecret"];
            if (string.IsNullOrWhiteSpace(endpointSecret))
                return Error.Failure("Webhook secret missing", "Stripe webhook secret is not configured");

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(request, stripeSignure, endpointSecret);
            }
            catch (StripeException ex)
            {
                return Error.Validation("Invalid Stripe signature", ex.Message);
            }

            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent is null)
                return Result.Ok();

            if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
                return await orderService.UpdatePaymentStatusByIntentIdAsync(paymentIntent.Id, true);

            if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
                return await orderService.UpdatePaymentStatusByIntentIdAsync(paymentIntent.Id, false);

            return Result.Ok();
        }
    }
}
