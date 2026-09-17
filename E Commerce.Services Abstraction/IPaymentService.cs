using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Shared.CommonResult;
using E_Commerce.Shared.DTOS.BasketDTOS;

namespace E_Commerce.Services_Abstraction
{
    public interface IPaymentService
    {
        Task<Result<CartDto>> CreateOrUpdatePaymentIntentAsync(string basketId);
        Task<Result> UpdateOrderPaymentSucceededAsync(string request, string stripeSignure);
    }
}
