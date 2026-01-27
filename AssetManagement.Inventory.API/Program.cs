using AssetManagement.Inventory.API.Domain.Constants;
using AssetManagement.Inventory.API.Domain.Entities.Identity;
using AssetManagement.Inventory.API.Infrastructure.Data;
using AssetManagement.Inventory.API.Infrastructure.Middlewares;
using AssetManagement.Inventory.API.Infrastructure.Middlewares.AssetManagement.Inventory.API.Middlewares;
using AssetManagement.Inventory.API.Infrastructure.Seed;
using AssetManagement.Inventory.API.Messaging.Consumers;
using AssetManagement.Inventory.API.Messaging.RabbitMQ;
using AssetManagement.Inventory.API.Services.Auth.Implementations;
using AssetManagement.Inventory.API.Services.Auth.Interfaces;
using AssetManagement.Inventory.API.Services.Discard.Implementations;
using AssetManagement.Inventory.API.Services.Discard.Interfaces;
using AssetManagement.Inventory.API.Services.Email.Implementations;
using AssetManagement.Inventory.API.Services.Email.Interfaces;
using AssetManagement.Inventory.API.Services.Implementations;
using AssetManagement.Inventory.API.Services.Interfaces;
using AssetManagement.Inventory.API.Services.Notification.Implementations;
using AssetManagement.Inventory.API.Services.Notification.Interface;
using AssetManagement.Inventory.API.Validators.Area;
using AssetManagement.Inventory.API.Validators.Environment;
using AssetManagement.Inventory.API.Validators.Item;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using AssetManagement.Inventory.API.Infrastructure.Settings;
using DotNetEnv;

Env.Load();

// 🔒 VALIDAÇÃO DE VARIÁVEIS DE AMBIENTE (FAIL FAST)
string RequireEnv(string key) =>
    Environment.GetEnvironmentVariable(key)
    ?? throw new InvalidOperationException(
        $"Variável de ambiente '{key}' não definida"
    );

RequireEnv("DB_HOST");
RequireEnv("DB_PORT");
RequireEnv("DB_NAME");
RequireEnv("DB_USER");
RequireEnv("DB_PASSWORD");

RequireEnv("EMAIL_FROM");
RequireEnv("EMAIL_SMTP");
RequireEnv("EMAIL_PORT");
RequireEnv("EMAIL_USER");
RequireEnv("EMAIL_PASSWORD");

RequireEnv("JWT_KEY");
RequireEnv("JWT_ISSUER");
RequireEnv("JWT_AUDIENCE");

var builder = WebApplication.CreateBuilder(args);

// SERVICES
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEnvironmentService, EnvironmentService>();
builder.Services.AddHostedService<ItemDiscardRequestedConsumer>();
builder.Services.AddScoped<IItemDiscardRequestService, ItemDiscardRequestService>();

//EMAIL
builder.Services.Configure<EmailSettings>(options =>
{
    options.From = Environment.GetEnvironmentVariable("EMAIL_FROM");
    options.Smtp = Environment.GetEnvironmentVariable("EMAIL_SMTP");
    options.Port = int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT")!);
    options.User = Environment.GetEnvironmentVariable("EMAIL_USER");
    options.Password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
});




builder.Environment.ContentRootPath = Directory.GetCurrentDirectory();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//VALIDAÇÃO COM FLUENTVALIDATOR
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateEnvironmentValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateAreaValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateItemValidator>();

//SERVIÇO DE MENSSAGERIA
builder.Services.Configure<RabbitMqSettings>(
    builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddScoped<IRabbitMqPublisher, RabbitMqPublisher>();
builder.Services.AddHostedService<TermResponsibilityUploadedConsumer>();
builder.Services.AddScoped<INotificationService, NotificationService>();




// DATABASE
var connectionString =
    $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
    $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
    $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
    $"User Id={Environment.GetEnvironmentVariable("DB_USER")};" +
    $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
    $"Pooling=true;";

builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseNpgsql(connectionString));


// IDENTITY
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<InventoryDbContext>()
.AddDefaultTokenProviders();

// JWT AUTHENTICATION
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")!;
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

var key = Encoding.UTF8.GetBytes(jwtKey);


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// AUTHORIZATION (ROLES)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireMaster", policy =>
        policy.RequireRole(Roles.Master));

    options.AddPolicy("RequireAdmin", policy =>
        policy.RequireRole(Roles.Admin, Roles.Master));

    options.AddPolicy("RequireUser", policy =>
        policy.RequireRole(Roles.User, Roles.Admin, Roles.Master));
});

// CONTROLLERS + SWAGGER
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Asset API", Version = "v1" });

    // JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Digite: Bearer {seu token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});


var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// await IdentitySeed.SeedAsync(app.Services); //se tiver que rodar para gerar as roles e o primeiro usuário Master

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization(); 

app.MapControllers();
app.Run();
