using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using E_Commerce.Domain.Contracts;
using E_Commerce.Services_Abstraction;

namespace E_Commerce.Services
{
    public class CacheService : ICacheService
    {
        private readonly ICacheRepository cacheRepository;
        public CacheService(ICacheRepository _cacheRepository)
        {
            cacheRepository = _cacheRepository;
        }
        public async Task<string?> GetAsync(string Cachekey)
        {
            return await cacheRepository.GetAsync(Cachekey);
        }

        public async Task SetAsync(string key, object CacheValue, TimeSpan timeSpan)
        {
            var value = JsonSerializer.Serialize(CacheValue, new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            await cacheRepository.SetAsync(key, value, timeSpan);
        }
    }
}
