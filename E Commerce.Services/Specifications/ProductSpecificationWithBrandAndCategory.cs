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
        public ProductSpecificationWithBrandAndCategory(ProductQueryParams queryParams) : 
            base(ProductSpacificationHelper.GetProductCritera(queryParams))
        {
            AddInclude(x => x.Brand);
            AddInclude(x => x.Category);

            switch (queryParams.Sort)
            {
                case ProductSortingOptions.NameAscending:
                    AddOrderAsc(p => p.Name);
                    break;
                case ProductSortingOptions.NameDescending:
                    AddOrderDec(p => p.Name);
                    break;
                case ProductSortingOptions.priceAscending:
                    AddOrderAsc(p => p.Price);
                    break;
                case ProductSortingOptions.priceDescending:
                    AddOrderDec(p => p.Price);
                    break;
                default:
                    AddOrderAsc(x => x.Id);
                    break;
            }
            ApplyPagination(queryParams.PageSize, queryParams.PageIndex);
        }
    }
}
