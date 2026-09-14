using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.CartModule;
using E_Commerce.Services_Abstraction;
using E_Commerce.Shared.DTOS.BasketDTOS;
using static E_Commerce.Services.Exceptions.NotFoundException;

namespace E_Commerce.Services
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository basketRepository;
        private readonly IMapper mapper;
        public BasketService(IBasketRepository basketRepository, IMapper mapper)
        {
            this.basketRepository = basketRepository;
            this.mapper = mapper;
        }
        public async Task<CartDto> CreateOrUpdateBasketAsync(CartDto basket)
        {
            var CustomerBasket = mapper.Map<Cart>(basket);
            var CreateorUpdate = await basketRepository.CreateOrUpdateBasketAsync(CustomerBasket);
            return mapper.Map<CartDto>(CreateorUpdate!);
        }

        public async Task<bool> DeleteBasketAsync(string basketId) => await basketRepository.DeleteBasketAsync(basketId);

        public async Task<CartDto> GetBasketAsync(string basketId)
        {
            var basket = await basketRepository.GetBasketAsync(basketId);
            if (basket == null) throw new BasketNotFoundException(basketId);
            return mapper.Map<CartDto>(basket!);
        }
    }
}
