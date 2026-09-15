using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Shared.DTOS.OrderDTOS
{
    public class OrderToReturnDto
    {

        public Guid Id { get; init; }
        public ICollection<OrderItemDto> Items { get; init; } = default!;
        public AddressDto Address { get; init; } = default!;
        public string DeliveryMethod { get; init; } = default!;
        public string OrderStatus { get; init; } = default!;
        public DateTime OrderDate { get; init; }
        public decimal SubTotal { get; init; }
        public decimal Total { get; init; }

    }
}
