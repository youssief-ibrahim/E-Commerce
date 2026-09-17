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
            AddIncludes();
            AddOrderDec(e => e.OrderDate);
        }
        public OrderSpecification(Guid id) : base(e => e.Id == id)
        {
            AddIncludes();
        }
        public OrderSpecification(Guid id,string userId) : base(e =>e.Id==id && e.UserId == userId)
        {
            AddIncludes();
        }

        private void AddIncludes()
        {
            AddInclude(e => e.DeliveryMethod);
            AddInclude(e => e.Items);
        }
    }
}
