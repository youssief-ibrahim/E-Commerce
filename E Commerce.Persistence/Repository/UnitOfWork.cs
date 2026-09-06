using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Domain;
using E_Commerce.Domain.Contracts;
using E_Commerce.Persistence.Data.DbContext;

namespace E_Commerce.Persistence.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EcomerceDbContext dbContext;
        private readonly Dictionary<Type, object> _repositories = [];

        public UnitOfWork(EcomerceDbContext _dbContext)
        {
            dbContext = _dbContext;
        }
        public IGenericRepository<T, TKey> GetRepository<T, TKey>() where T : BaseEntity<TKey>
        {
            var type = typeof(T);
            if (_repositories.TryGetValue(type, out object? repository))
            {
                return (IGenericRepository<T, TKey>)repository!;
            }
            var newRepository = new GenericRepository<T, TKey>(dbContext);
            _repositories[type] = newRepository;
            return newRepository;
        }

        public async Task<int> SaveChangeAsync()=> await dbContext.SaveChangesAsync();
        
    }
}
