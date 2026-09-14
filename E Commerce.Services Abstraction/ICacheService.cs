using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Services_Abstraction
{
    public interface ICacheService
    {
        Task<string?> GetAsync(string Cachekey);
        Task SetAsync(string key, object CacheValue, TimeSpan timeSpan);
    }
}
