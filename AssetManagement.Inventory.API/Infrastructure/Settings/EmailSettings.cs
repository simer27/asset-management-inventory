namespace AssetManagement.Inventory.API.Infrastructure.Settings
{
    public class EmailSettings
    {
        public string? From { get; set; }
        public string? Smtp { get; set; }
        public int Port { get; set; }
        public string? User { get; set; }
        public string? Password { get; set; }
    }
}
