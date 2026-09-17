using E_Commerce.Services_Abstraction;
using E_Commerce.Shared.DTOS.BasketDTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Presentation.Controllers
{
    public class PaymentController : ApiBaseController
    {
        private readonly IPaymentService paymentService;

        public PaymentController(IPaymentService _paymentService)
        {
            paymentService = _paymentService;
        }

        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CartDto>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var res = await paymentService.CreateOrUpdatePaymentIntentAsync(basketId);
            return HandleResult<CartDto>(res);
        }

        [AllowAnonymous]
        [HttpPost("Webhook")]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"].ToString();

            var result = await paymentService.UpdateOrderPaymentSucceededAsync(json, stripeSignature);

            return HandleResult(result);
        }
    }
}
