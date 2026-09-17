using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Domain.Entities.OrdersModule;

namespace E_Commerce.Services.Specifications
{
    public class OrderwithPaymentIntentSpecefication : BaseSpecifications<Order, Guid>
    {
        public OrderwithPaymentIntentSpecefication(string paymentIntentId):base(e=>e.PaymentIntentId==paymentIntentId)
        {
            
        }
    }
}
