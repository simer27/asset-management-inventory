using AssetManagement.Inventory.API.Domain.Entities;
using AssetManagement.Inventory.API.Domain.Entities.Identity;
using AssetManagement.Inventory.API.Infrastructure.Data;
using AssetManagement.Inventory.API.Messaging.Events;
using AssetManagement.Inventory.API.Services.Email.Interfaces;
using Microsoft.AspNetCore.Identity;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace AssetManagement.Inventory.API.Messaging.Consumers
{
    public class ItemDiscardRequestedConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ItemDiscardRequestedConsumer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: "item-discard-requested",
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (_, ea) =>
            {
                using var scope = _scopeFactory.CreateScope();

                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var context = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

                var message = JsonSerializer.Deserialize<ItemDiscardRequestedEvent>(
                    Encoding.UTF8.GetString(ea.Body.ToArray())
                );

                if (message == null)
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                    return;
                }

                var admins = await userManager.GetUsersInRoleAsync("Admin");

                foreach (var admin in admins)
                {
                    var notification = new Notification
                    {
                        UserId = admin.Id,
                        Title = "Aprovação de descarte pendente",
                        Message = $"Existe uma solicitação de descarte pendente.\n" +
                                  $"Item: {message.ItemName}\n" +
                                  $"Área: {message.AreaName}\n" +
                                  $"Solicitado por: {message.RequestedBy}\n" +
                                  "Por favor, não responda este e-mail.",
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false
                    };

                    context.Notifications.Add(notification);

                    await emailService.SendAsync(
                        admin.Email!,
                        notification.Title,
                        notification.Message
                    );
                }

                await context.SaveChangesAsync();
                channel.BasicAck(ea.DeliveryTag, false);
            };

            channel.BasicConsume("item-discard-requested", false, consumer);

            return Task.CompletedTask;
        }
    }
}
