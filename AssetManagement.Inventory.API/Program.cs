using AssetManagement.Inventory.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Inventory.API.Services.Interfaces;
using AssetManagement.Inventory.API.Services.Implementations;
using Microsoft.AspNetCore.Identity;
using AssetManagement.Inventory.API.Domain.Entities.Identity;
using AssetManagement.Inventory.API.Services.Auth.Implementations;
using AssetManagement.Inventory.API.Services.Auth.Interfaces;
using AssetManagement.Inventory.API.Services.Email.Implementations;
using AssetManagement.Inventory.API.Services.Email.Interfaces;

namespace AssetManagement.Inventory.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Adicione serviços ao contêiner.
            builder.Services.AddScoped<IAreaService, AreaService>();
            builder.Services.AddScoped<IItemService, ItemService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IEmailService, EmailService>();


            // Configurar o Entity Framework com PostgreSQL
            builder.Services.AddDbContext<InventoryDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<InventoryDbContext>()
            .AddDefaultTokenProviders();



            builder.Services.AddControllers();

            // Aprenda mais sobre a configuração do Swagger/OpenAPI em https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configurar o pipeline de solicitação HTTP.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication(); // Adicione esta linha para habilitar a autenticação
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}