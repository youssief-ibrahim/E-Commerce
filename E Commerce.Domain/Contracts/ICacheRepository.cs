using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Contracts
{
    public interface ICacheRepository
    {
        Task<string?> GetAsync(string key);
        Task SetAsync(string key, string CacheValue, TimeSpan timeSpan);
    }
}
