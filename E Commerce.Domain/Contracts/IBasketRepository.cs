using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Domain.Entities.CartModule;

namespace E_Commerce.Domain.Contracts
{
    public interface IBasketRepository
    {
        Task<Cart?> GetBasketAsync(string basketId);
        Task<Cart?> CreateOrUpdateBasketAsync(Cart basket, TimeSpan time = default);
        Task<bool> DeleteBasketAsync(string basketId);
    }
}
