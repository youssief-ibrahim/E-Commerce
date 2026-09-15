using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Domain.Entities.OrdersModule;

namespace E_Commerce.Services.Specifications
{
    public class OrderSpecification : BaseSpecifications<Order, Guid>
    {
        public OrderSpecification(string userId) : base(e=>e.UserId== userId)
        {
            AddInclude(e => e.DeliveryMethod);
            AddInclude(e=>e.Items);
        }
        public OrderSpecification(Guid id,string userId) : base(e =>e.Id==id && e.UserId == userId)
        {
            AddInclude(e => e.DeliveryMethod);
            AddInclude(e => e.Items);
        }
    }
}
