using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Domain;
using E_Commerce.Domain.Contracts;
using E_Commerce.Persistence.Data.DbContext;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Persistence.Repository
{
    public class GenericRepository<T, TKey> : IGenericRepository<T, TKey> where T : BaseEntity<TKey>
    {
        private readonly EcomerceDbContext dbContext;
        public GenericRepository(EcomerceDbContext _dbContext)
        {
            dbContext = _dbContext;
        }
        public async Task<IEnumerable<T>> GetAllAsync() => await dbContext.Set<T>().ToListAsync();
        public async Task<T?> GetByIdAsync(TKey id) => await dbContext.Set<T>().FindAsync(id);
        public async Task AddAsync(T entity) => await dbContext.Set<T>().AddAsync(entity);
        public void Update(T entity) => dbContext.Set<T>().Update(entity);
        public void Delete(T entity) => dbContext.Set<T>().Remove(entity);
    }
}
