using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Contracts
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangeAsync();
        IGenericRepository<T, TKey> GetRepository<T, TKey>() where T : BaseEntity<TKey>;
    }
}
