using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Entities.OrdersModule
{
    public enum OrderStatus
    {
        Pending,
        PaymentReceived,
        PaymentFailed,
        Shipped,
        Delivered,
        Cancelled
    }
}
