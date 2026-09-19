using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Domain.Entities.IdentityModule
{
    public class RefreshToken : BaseEntity<Guid>
    {
        public string TokenHash { get; set; } = default!;
        public DateTime ExpiresOn { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? RevokedOn { get; set; }
        public string? ReplacedByTokenHash { get; set; }
        public string? CreatedByIp { get; set; }

        [NotMapped]
        public bool IsExpired => DateTime.Now >= ExpiresOn;

        [NotMapped]
        public bool IsActive => RevokedOn is null && !IsExpired;

        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;
    }
}
