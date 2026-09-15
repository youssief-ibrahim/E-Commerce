using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.ProductModule;
using E_Commerce.Services.Specifications;
using E_Commerce.Services_Abstraction;
using E_Commerce.Shared;
using E_Commerce.Shared.CommonResult;
using E_Commerce.Shared.DTOS.ProductDTOS;
using static E_Commerce.Services.Exceptions.NotFoundException;

namespace E_Commerce.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public ProductService(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }

        public async Task<Result<ProductDto>> CreateProductAsync(CreateOrUpdateProductDto productDto)
        {
            var brand=await unitOfWork.GetRepository<Product,int>().GetByIdAsync(productDto.BrandId);
            if (brand == null) return Error.NotFound($"Brand Not Found ", $"Brand with {productDto.BrandId} is Not Found");
            var category = await unitOfWork.GetRepository<Product, int>().GetByIdAsync(productDto.CategoryId);
            if (category == null) return Error.NotFound($"Category Not Found ", $"Category with {productDto.CategoryId} is Not Found");
             
            
            var product = mapper.Map<Product>(productDto);

            await unitOfWork.GetRepository<Product, int>().AddAsync(product);

            await unitOfWork.SaveChangeAsync();

            return await GetProductByIdAsync(product.Id);
        }

        public async Task<Result> DeleteProductAsync(int id)
        {
            var productToDelete = await unitOfWork.GetRepository<Product, int>() .GetByIdAsync(id);

            if (productToDelete == null) return Error.NotFound("Product Not Found", $"Product with {id} is Not Found");

            unitOfWork.GetRepository<Product, int>().Delete(productToDelete);

            await unitOfWork.SaveChangeAsync();

            return Result.Ok();
        }

        public async Task<PaginatedResult<ProductDto>> GetAllProductAsync(ProductQueryParams queryParams)
        {
            var specific = new ProductSpecificationWithBrandAndCategory(queryParams);
            var products = await unitOfWork.GetRepository<Product, int>().GetAllWithSpecificationAsync(specific);
            var DataToReturn= mapper.Map<IReadOnlyList<ProductDto>>(products);
            var CountOfReturnedData = DataToReturn.Count;

            var specificOfCount = new ProductCountSpecification(queryParams);
            var CountOfAllProduct=await unitOfWork.GetRepository<Product, int>().CountAsync(specificOfCount);

            return new PaginatedResult<ProductDto>(CountOfReturnedData, queryParams.PageIndex, CountOfAllProduct, DataToReturn);
        }
        public async Task<Result<ProductDto>> GetProductByIdAsync(int id)
        {
            var specific = new ProductSpecificationWithBrandAndCategory(id);
            var product = await unitOfWork.GetRepository<Product, int>().GetByIdWithSpecificationAsync(specific);
            if (product == null)
                return Error.NotFound($"Product Not Found ", "Product with {id} is Not Found");
            return mapper.Map<ProductDto>(product);
            #region BeFor ImplicitCast
            //    return Result<ProductDto>.Fail(Error.NotFound($"Product Not Found ", "Product with {id} is Not Found"))!;
            //return Result<ProductDto>.Ok(mapper.Map<ProductDto>(product));
            #endregion
        }

        public async Task<Result> UpdateProductAsync(int id, CreateOrUpdateProductDto product)
        {
            var productToUpdate = await unitOfWork.GetRepository<Product, int>().GetByIdAsync(id);

            if (productToUpdate == null)
              return Error.NotFound( "Product Not Found", $"Product with {id} is Not Found");

            var brand = await unitOfWork.GetRepository<Brand, int>().GetByIdAsync(product.BrandId);

            if (brand == null) return Error.NotFound("Brand Not Found", $"Brand with {product.BrandId} is Not Found");

            var category = await unitOfWork .GetRepository<Category, int>() .GetByIdAsync(product.CategoryId);

            if (category == null) return Error.NotFound("Category Not Found", $"Category with {product.CategoryId} is Not Found");

            // Update 
            mapper.Map(product, productToUpdate);

            unitOfWork.GetRepository<Product, int>().Update(productToUpdate);

            await unitOfWork.SaveChangeAsync();

            // Get the updated product with Brand & Category
            //return await GetProductByIdAsync(productToUpdate.Id);
           return Result.Ok();
        }
    }
}
