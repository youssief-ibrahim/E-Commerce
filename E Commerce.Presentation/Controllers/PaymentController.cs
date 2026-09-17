using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Services_Abstraction;
using E_Commerce.Shared.DTOS.BasketDTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace E_Commerce.Presentation.Controllers
{
    public class PaymentController : ApiBaseController
    {
        private readonly IPaymentService paymentService;
        private readonly IConfiguration configuration;

        public PaymentController(IPaymentService _paymentService, IConfiguration _configuration)
        {
            paymentService = _paymentService;
            configuration = _configuration;
        }

        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CartDto>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var res = await paymentService.CreateOrUpdatePaymentIntentAsync(basketId);
            return HandleResult<CartDto>(res);
        }
        [HttpPost("Weebhook")]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];

            await paymentService.UpdateOrderPaymentSucceededAsync(json, stripeSignature!);

            return new EmptyResult();
        }
    }
}
