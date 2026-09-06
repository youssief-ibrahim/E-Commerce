using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Domain.Entities.ProductModule;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Domain.Entities.OrdersModule
{
    public class OrderItem : BaseEntity<int>
    {
        public ProductItemOrdered product { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }


        #region RelationShip
        public Guid OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; } = default!;
        #endregion
    }
    [Owned]
    public class ProductItemOrdered
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string PictureUrl { get; set; } = null!;
    }
}
