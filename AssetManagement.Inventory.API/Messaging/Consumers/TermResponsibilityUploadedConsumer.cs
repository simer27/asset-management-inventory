using AssetManagement.Inventory.API.DTOs.Messaging;
using AssetManagement.Inventory.API.Messaging.RabbitMQ;
using AssetManagement.Inventory.API.Services.Email.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace AssetManagement.Inventory.API.Messaging.Consumers
{
    public class TermResponsibilityUploadedConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMqSettings _settings;

        public TermResponsibilityUploadedConsumer(
            IServiceProvider serviceProvider,
            IOptions<RabbitMqSettings> options)
        {
            _serviceProvider = serviceProvider;
            _settings = options.Value;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(_settings.QueueName, true, false, false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (_, e) =>
            {
                var body = Encoding.UTF8.GetString(e.Body.ToArray());
                var message = JsonSerializer.Deserialize<TermResponsibilityUploadedEvent>(body)!;

                using var scope = _serviceProvider.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                await emailService.SendTermResponsibilityToAdminsAsync(message);
            };

            channel.BasicConsume(_settings.QueueName, true, consumer);

            return Task.CompletedTask;
        }
    }
}
