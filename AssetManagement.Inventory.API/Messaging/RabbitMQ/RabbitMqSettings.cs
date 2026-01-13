namespace AssetManagement.Inventory.API.Messaging.RabbitMQ
{
    public class RabbitMqSettings
    {
        public string HostName { get; set; } = "localhost";
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string QueueName { get; set; } = "term-responsibility-uploaded";
    }
}
