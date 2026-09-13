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
using E_Commerce.Shared.DTOS.ProductDTOS;

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
        public async Task<IReadOnlyList<ProductDto>> GetAllProductAsync(ProductQueryParams queryParams)
        {
            var specific = new ProductSpecificationWithBrandAndCategory(queryParams);
            var products = await unitOfWork.GetRepository<Product, int>().GetAllWithSpecificationAsync(specific);
            return mapper.Map<IReadOnlyList<ProductDto>>(products);
        }
        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var specific = new ProductSpecificationWithBrandAndCategory(id);
            var product = await unitOfWork.GetRepository<Product, int>().GetByIdAsync(specific);
            return mapper.Map<ProductDto>(product);
        }
    }
}
