using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Domain.Entities.OrdersModule;

namespace E_Commerce.Domain.Entities.ProductModule
{
    public class Product: BaseEntity<int>
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string PictureUrl { get; set; } = default!;


        #region RelationShip
        public int BrandId { get; set; }
        [ForeignKey("BrandId")]
        public Brand Brand { get; set; } = default!;

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = default!;

        #endregion
    }
}
