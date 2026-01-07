namespace AssetManagement.Inventory.API.Domain.Entities
{
    public class EnvironmentImage
    {
        public Guid Id { get; set; }

        public string FileName { get; set; }
        public string FilePath { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid EnvironmentId { get; set; }
        public EnvironmentEntity Environment { get; set; }
    }
}
