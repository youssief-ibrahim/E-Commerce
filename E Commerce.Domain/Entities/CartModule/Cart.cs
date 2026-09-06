using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Entities.CartModule
{
    public class Cart
    {
        public string Id { get; set; }
        public ICollection<CartItem> Items { get; set; } = [];

    }
}
