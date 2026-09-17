using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Domain.Entities.IdentityModule;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Domain.Entities.OrdersModule
{
    public class Order : BaseEntity<Guid>
    {

        public DateTime OrderDate { get; set; } = DateTime.Now.Date;
        public OrderAddress Address { get; set; } = null!;
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public string? PaymentIntentId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal GetTotal() => Subtotal + DeliveryMethod.Price;

        #region RelationShip
        public string UserId { get; set; } = default!;
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = default!;

        public int DeliveryMethodId { get; set; }

        [ForeignKey("DeliveryMethodId")]
        public DeliveryMethod DeliveryMethod { get; set; } = null!;

        public ICollection<OrderItem> Items { get; set; } = [];
        #endregion
    }
    [Owned]
    public class OrderAddress
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
    }
}
