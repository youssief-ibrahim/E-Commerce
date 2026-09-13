using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Domain;
using E_Commerce.Domain.Contracts;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Persistence
{
    public static class SpecificationEvaluator
    {
        // context.include(p>p.Brand).include(p=>p.type)
        public static IQueryable<T> CreateQuery<T, TKey>(IQueryable<T> EntiryPoint, ISpecification<T, TKey> specification) where T : BaseEntity<TKey>
        {
            var query = EntiryPoint;

            if (specification != null)
            {
                if (specification.Criteria != null)
                {
                    query = query.Where(specification.Criteria);
                }

                if (specification.IncludeExpressions != null && specification.IncludeExpressions.Any())
                {
                    foreach (var expression in specification.IncludeExpressions)
                    {
                        query = query.Include(expression);
                    }

                    //  query = specification.IncludeExpressions.Aggregate(query, (current, includeEXP) => current.Include(includeEXP));
                }

                if (specification.OrderBy != null)
                {
                    query = query.OrderBy(specification.OrderBy);
                }

                if (specification.OrderByDescending != null)
                {
                    query = query.OrderByDescending(specification.OrderByDescending);
                }

                if (specification.IsPaginated)
                {
                    query = query.Skip(specification.Skip).Take(specification.Take);
                }
            }
            return query;
        }
    }
}
