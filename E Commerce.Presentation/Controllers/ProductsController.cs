using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Presentation.Attributes;
using E_Commerce.Services_Abstraction;
using E_Commerce.Shared;
using E_Commerce.Shared.DTOS.ProductDTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Presentation.Controllers
{
    public class ProductsController : ApiBaseController
    {
        private readonly IProductService productService;
        public ProductsController(IProductService _productService)
        {
            productService = _productService;
        }
        [HttpGet]
        [Authorize]
        [RedisCache]
        public async Task<ActionResult<PaginatedResult<ProductDto>>> GetAllProducts([FromQuery] ProductQueryParams queryParams)
        {
            var products = await productService.GetAllProductAsync(queryParams);
            return Ok(products);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var products = await productService.GetProductByIdAsync(id);
            return HandleResult<ProductDto>(products);
        }
        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateOrUpdateProductDto productDto)
        {
            var product = await productService.CreateProductAsync(productDto);
            return HandleResult<ProductDto>(product);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, CreateOrUpdateProductDto productDto)
        {
            var product = await productService.UpdateProductAsync(id, productDto);
            return HandleResult(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await productService.DeleteProductAsync(id);

            return HandleResult(result);
        }
    }
}
