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
        public PaymentService(IBasketRepository _basketRepository, IUnitOfWork _unitOfWork, IMapper _mapper, IConfiguration _config)
        {
            basketRepository = _basketRepository;
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            config = _config;
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
                var product = await unitOfWork.GetRepository<Product, int>().GetByIdAsync(item.Id);
                if (product is null) return Error.NotFound("Product not found", $"Product with id {item.Id} not found");

                item.ProductName = product.Name;
                item.PictureUrl = product.PictureUrl;
                item.Price = product.Price;
            }
            var shippingPrice = deliveryMethod.Price;
            var totalAmount = basket.Items.Sum(i => i.Price * i.Quantity) + shippingPrice;
            var amount = (long)(totalAmount * 100); // Convert to cents

            var paymentIntentService = new PaymentIntentService();
            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var Options = new PaymentIntentCreateOptions()
                {
                    Amount = amount,
                    Currency = "USD",
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
                await paymentIntentService.UpdateAsync(basket.PaymentIntentId, Options);
            }
            
            await basketRepository.CreateOrUpdateBasketAsync(basket);

            return mapper.Map<CartDto>(basket);
        }

        public async Task<Result> UpdateOrderPaymentSucceededAsync(string request, string stripeSignure)
        {
            var endpointSecret = config["EndpointSecret"];
            var stripeEvent = EventUtility.ConstructEvent(request, stripeSignure, endpointSecret);

            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;


            if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
            {
                var order = await unitOfWork.GetRepository<Order, Guid>().GetByIdWithSpecificationAsync(new OrderwithPaymentIntentSpecefication(paymentIntent.Id));
                if(order == null)
                   return Error.NotFound("Order not found for PaymentIntent ID: {0}", paymentIntent.Id);
                    
                
                order.OrderStatus = OrderStatus.PaymentReceived;
                await unitOfWork.SaveChangeAsync();
                return Result.Ok();

                // logic
            }
            else if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
            {

                var order = await unitOfWork.GetRepository<Order, Guid>().GetByIdWithSpecificationAsync(new OrderwithPaymentIntentSpecefication(paymentIntent.Id));
                if (order == null)
                    return Error.NotFound("Order not found for PaymentIntent ID: {0}", paymentIntent.Id);

                order.OrderStatus = OrderStatus.PaymentFailed;
                await unitOfWork.SaveChangeAsync();
                return Result.Ok(); 
                // logic
            }

            else
            {
                //Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                return Error.Failure("Unhandled event type: {0}", stripeEvent.Type);
            }
        }
    }
}
