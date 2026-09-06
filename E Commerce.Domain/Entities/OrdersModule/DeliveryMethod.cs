using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Entities.OrdersModule
{
    public class DeliveryMethod : BaseEntity<int>
    {
        public string ShortName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string DeliveryTime { get; set; } = null!;
        public decimal Price { get; set; }

        #region RelationShip
        public ICollection<Order> Orders { get; set; } = [];
        #endregion
    }
}
