using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Domain.Entities.OrdersModule;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Domain.Entities.IdentityModule
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = default!;

        #region RelationShip
        public ICollection<Order> Orders { get; set; } = [];
        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
        #endregion
    }
}
