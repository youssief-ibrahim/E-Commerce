using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Shared.DTOS.CartItemDTOS;

namespace E_Commerce.Shared.DTOS.BasketDTOS
{
    public record CartDto(
        string Id,
        ICollection<CartItemDto> Items,
        string? ClientSecret,
        string PaymentIntentId,
        int? DeliveryMethodId,
        decimal? ShippingPrice
        );
}