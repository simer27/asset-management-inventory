using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Inventory.API.Domain.Entities.Identity
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public bool IsRevoked { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;




        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }
}
