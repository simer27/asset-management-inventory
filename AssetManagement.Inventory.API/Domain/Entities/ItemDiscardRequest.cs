using System.ComponentModel.DataAnnotations;
using AssetManagement.Inventory.API.Domain.Entities.Identity;

namespace AssetManagement.Inventory.API.Domain.Entities
{
    public class ItemDiscardRequest
    {
        public Guid Id { get; set; }

        public Guid ItemId { get; set; }
        public Item Item { get; set; } = null!;

        public Guid RequestedByUserId { get; set; }
        public ApplicationUser RequestedBy { get; set; } = null!;

        public string Justification { get; set; } = string.Empty;

        public bool Approved { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
