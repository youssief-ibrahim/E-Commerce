using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Shared.DTOS.OrderDTOS
{
    public class DeliveryMethodDto
    {
        public string ShortName { get; init; } = null!;
        public string Description { get; init; } = null!;
        public string DeliveryTime { get; init; } = null!;
        public decimal Price { get; init; }
    }
}
