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
    public class ProductSpecificationWithBrandAndCategory : BaseSpecifications<Product, int>
    {
        // get Product By iD
        public ProductSpecificationWithBrandAndCategory(int id) : base(P=>P.Id==id)
        {
            AddInclude(x => x.Brand);
            AddInclude(x => x.Category);
        }
        // GetAll Product
        public ProductSpecificationWithBrandAndCategory(ProductQueryParams queryParams) : base(x =>
            (string.IsNullOrEmpty(queryParams.Search) || x.Name.ToLower().Contains(queryParams.Search.ToLower())) &&
            (!queryParams.BrandId.HasValue || x.BrandId == queryParams.BrandId) &&
            (!queryParams.CategoryId.HasValue || x.CategoryId == queryParams.CategoryId))
        {
            AddInclude(x => x.Brand);
            AddInclude(x => x.Category);
        }
    }
}
