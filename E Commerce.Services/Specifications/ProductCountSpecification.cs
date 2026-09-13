using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Domain.Entities.ProductModule;
using E_Commerce.Shared;

namespace E_Commerce.Services.Specifications
{
    public class ProductCountSpecification : BaseSpecifications<Product, int>
    {
        public ProductCountSpecification(ProductQueryParams queryParams) :
          base(ProductSpacificationHelper.GetProductCritera(queryParams))
        {
            AddInclude(x => x.Brand);
            AddInclude(x => x.Category);
        }
    }
}
