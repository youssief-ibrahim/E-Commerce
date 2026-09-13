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
    public static class ProductSpacificationHelper
    {
        public static Expression<Func<Product, bool>> GetProductCritera(ProductQueryParams queryParams)
        {
            return x =>
            (string.IsNullOrEmpty(queryParams.Search) || x.Name.ToLower().Contains(queryParams.Search.ToLower())) &&
            (!queryParams.BrandId.HasValue || x.BrandId == queryParams.BrandId) &&
            (!queryParams.CategoryId.HasValue || x.CategoryId == queryParams.CategoryId);
        }
    }
}
