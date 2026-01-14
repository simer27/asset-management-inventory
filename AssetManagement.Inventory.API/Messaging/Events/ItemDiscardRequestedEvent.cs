namespace AssetManagement.Inventory.API.Messaging.Events
{
    public class ItemDiscardRequestedEvent
    {
        public Guid DiscardRequestId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string AreaName { get; set; } = string.Empty;
        public string RequestedBy { get; set; } = string.Empty;
        public string Justification { get; set; } = string.Empty;
    }

}
