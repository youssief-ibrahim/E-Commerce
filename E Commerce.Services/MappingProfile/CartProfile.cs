using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using E_Commerce.Domain.Entities.CartModule;
using E_Commerce.Shared.DTOS.BasketDTOS;
using E_Commerce.Shared.DTOS.CartItemDTOS;

namespace E_Commerce.Services.MappingProfile
{
    public class CartProfile:Profile
    {
        public CartProfile()
        {
            CreateMap<Cart, CartDto>().ReverseMap();
            CreateMap<CartItem, CartItemDto>().ReverseMap();
        }
    }
}
