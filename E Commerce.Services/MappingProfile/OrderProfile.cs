using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using E_Commerce.Domain.Entities.OrdersModule;
using E_Commerce.Shared.DTOS.OrderDTOS;

namespace E_Commerce.Services.MappingProfile
{
    public class OrderProfile:Profile
    {
        public OrderProfile()
        {
            CreateMap<AddressDto, OrderAddress>().ReverseMap();

            CreateMap<Order, OrderToReturnDto>()
                 .ForMember(d => d.DeliveryMethod, o => o.MapFrom(s => s.DeliveryMethod.ShortName))
                 .ForMember(d => d.OrderStatus, o => o.MapFrom(s => s.OrderStatus.ToString()))
                 .ForMember(d => d.SubTotal, o => o.MapFrom(s => s.Subtotal))
                 .ForMember(d => d.Total, o => o.MapFrom(s => s.Subtotal + s.DeliveryMethod.Price));

            CreateMap<OrderItem, OrderItemDto>()
                 .ForMember(d => d.ProductId, o => o.MapFrom(s => s.product.ProductId))
                 .ForMember(d => d.ProductName, o => o.MapFrom(s => s.product.ProductName))
                 .ForMember(d => d.PictureUrl, o => o.MapFrom<PictureURLResolver<OrderItem, OrderItemDto>, string>(src => src.product.PictureUrl));

            CreateMap<DeliveryMethod,DeliveryMethodDto>();


            CreateMap<OrderStatusDto, OrderStatus>();

        }
    }
}
