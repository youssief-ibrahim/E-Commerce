using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Shared;
using E_Commerce.Shared.DTOS.ProductDTOS;

namespace E_Commerce.Services_Abstraction
{
    public interface IProductService
    {
        //Task<IReadOnlyList<ProductDto>> GetProductsAsync(ProductQueryParams queryParams);
        Task<PaginatedResult<ProductDto>> GetAllProductAsync(ProductQueryParams queryParams);
        Task<ProductDto?> GetProductByIdAsync(int id);
        //Task<Product> CreateProductAsync(Product product);
        //Task UpdateProductAsync(Product product);
        //Task DeleteProductAsync(int id);
    }
}
