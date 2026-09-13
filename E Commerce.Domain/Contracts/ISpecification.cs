using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Contracts
{
    public interface ISpecification<T, TKey> where T :BaseEntity<TKey>
    {
        public ICollection<Expression<Func<T, object>>> IncludeExpressions { get; }
        public Expression<Func<T, bool>> Criteria { get; }
        public Expression<Func<T, object>> OrderBy { get; }
        public Expression<Func<T, object>> OrderByDescending { get; }
        public int Take { get; }
        public int Skip { get; }
        public bool IsPaginated { get; }
    }
}
