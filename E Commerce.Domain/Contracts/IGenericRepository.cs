using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Contracts
{
    public interface IGenericRepository<T, TKey> where T : BaseEntity<TKey>
    {
        Task<IEnumerable<T>> GetAllAsync();
        //Task<IEnumerable<T>> GetAllAsync(ISpecification<T, TKey> specification);
        Task<T?> GetByIdAsync(TKey id);
        //Task<T?> GetByIdAsync(ISpecification<T, TKey> specification);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        //Task<int> CountAsync(ISpecification<T, TKey> specification);
    }
}
