using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Shared.DTOS.BasketDTOS;

namespace E_Commerce.Services_Abstraction
{
    public interface IBasketService
    {
        Task<CartDto> GetBasketAsync(string basketId);
        Task<CartDto> CreateOrUpdateBasketAsync(CartDto basket);
        Task<bool> DeleteBasketAsync(string basketId);
    }
}
