using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.CartModule;
using StackExchange.Redis;

namespace E_Commerce.Persistence.Repository
{
    // Redis 
    public class BasketRepository : IBasketRepository
    {
         private readonly IDatabase database;

        public BasketRepository(IConnectionMultiplexer connection)
        {
            database=connection.GetDatabase();
        }
        public async Task<Cart?> CreateOrUpdateBasketAsync(Cart basket, TimeSpan time = default)
        {
            var basketjson = JsonSerializer.Serialize(basket);
            var CreeateorUpdate = await database.StringSetAsync(basket.Id, basketjson, (time == default) ? TimeSpan.FromDays(7) : time);
            if (CreeateorUpdate)
            {
                return await GetBasketAsync(basket.Id);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> DeleteBasketAsync(string basketId) => await database.KeyDeleteAsync(basketId);
  
        public async Task<Cart?> GetBasketAsync(string basketId)
        {
            var basket = await database.StringGetAsync(basketId);
            if (basket.IsNullOrEmpty)
                return null;

            return JsonSerializer.Deserialize<Cart>(basket!);
        }
    }
}
