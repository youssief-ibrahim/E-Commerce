using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Domain.Contracts;
using StackExchange.Redis;

namespace E_Commerce.Persistence.Repository
{
    public class CacheRepository : ICacheRepository
    {
        private readonly IDatabase database;
        public CacheRepository(IConnectionMultiplexer connection)
        {
            database = connection.GetDatabase();
        }

        public Task DeleteAsync(string key)
        {
            return database.KeyDeleteAsync(key);
        }

        public async Task<string?> GetAsync(string key)
        {
            var CacheResult = await database.StringGetAsync(key);
            return CacheResult.IsNullOrEmpty ? null : CacheResult.ToString();
        }

        public async Task SetAsync(string key, string CacheValue, TimeSpan timeSpan)
        {
            await database.StringSetAsync(key, CacheValue, timeSpan);
        }
    }
}
