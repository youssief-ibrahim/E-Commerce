using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Domain;
using E_Commerce.Domain.Contracts;

namespace E_Commerce.Services.Specifications
{
    public abstract  class BaseSpecifications<T, TKey> : ISpecification<T, TKey> where T : BaseEntity<TKey>
    {
        #region Criteria
        public Expression<Func<T, bool>> Criteria { get; }
        //protected BaseSpecifications() { } 
        protected BaseSpecifications(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }
        #endregion

        #region Include
        public ICollection<Expression<Func<T, object>>> IncludeExpressions { get; } = [];
        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            IncludeExpressions.Add(includeExpression);
        }
        #endregion

        #region Sorting
        public Expression<Func<T, object>> OrderBy { get; private set; }

        public Expression<Func<T, object>> OrderByDescending { get; private set; }
        protected void AddOrderAsc(Expression<Func<T, object>> Orderasc)
        {
            OrderBy = Orderasc;
        }
        protected void AddOrderDec(Expression<Func<T, object>> OrderDecc)
        {
            OrderByDescending = OrderDecc;
        }
        #endregion
        #region Pagination

        public int Take { get; private set; }

        public int Skip { get; private set; }

        public bool IsPaginated { get; private set; }
        // 13  size=4 index=2
        protected void ApplyPagination(int PageSize, int PageIndex)
        {
            IsPaginated = true;
            Take = PageSize;
            Skip = (PageIndex - 1) * PageSize;
        }
        #endregion
    }
}
