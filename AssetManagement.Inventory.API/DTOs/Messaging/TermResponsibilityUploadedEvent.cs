namespace AssetManagement.Inventory.API.DTOs.Messaging
{
    public class TermResponsibilityUploadedEvent
    {
        public Guid DocumentId { get; set; }
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public DateTime UploadedAt { get; set; }
    }
}
