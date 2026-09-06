using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Entities.ProductModule
{
    public class Brand: BaseEntity<int>
    {
        public string Name { get; set; } = default!;


        #region RelationShip
        public ICollection<Product> Products { get; set; } = [];
        #endregion
    }
}
