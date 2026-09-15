using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Shared;
using E_Commerce.Shared.CommonResult;
using E_Commerce.Shared.DTOS.ProductDTOS;

namespace E_Commerce.Services_Abstraction
{
    public interface IProductService
    {
        //Task<IReadOnlyList<ProductDto>> GetProductsAsync(ProductQueryParams queryParams);
        Task<PaginatedResult<ProductDto>> GetAllProductAsync(ProductQueryParams queryParams);
        Task<Result<ProductDto>> GetProductByIdAsync(int id);
        Task<Result<ProductDto>> CreateProductAsync(CreateOrUpdateProductDto product);
        Task<Result> UpdateProductAsync(int id,CreateOrUpdateProductDto product);
        Task<Result> DeleteProductAsync(int id);
    }
}
