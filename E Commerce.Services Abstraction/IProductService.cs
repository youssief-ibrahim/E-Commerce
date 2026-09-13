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
        Task<IEnumerable<ProductDto>> GetAllProductAsync(ProductQueryParams queryParams);
        //Task<PaginatedResult<ProductDto>> GetProductsAsync(ProductQueryParams queryParams);
        //Task<Product?> GetProductByIdAsync(int id);
        //Task<Product> CreateProductAsync(Product product);
        //Task UpdateProductAsync(Product product);
        //Task DeleteProductAsync(int id);
    }
}
