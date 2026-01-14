namespace AssetManagement.Inventory.API.Messaging.RabbitMQ
{
    public interface IRabbitMqPublisher
    {
        void Publish<T>(T message, string queueName);
    }
}
