using AssetManagement.Inventory.API.Domain.Constants;
using AssetManagement.Inventory.API.Domain.Entities.Identity;
using AssetManagement.Inventory.API.Infrastructure.Data;
using AssetManagement.Inventory.API.Infrastructure.Middlewares;
using AssetManagement.Inventory.API.Infrastructure.Middlewares.AssetManagement.Inventory.API.Middlewares;
using AssetManagement.Inventory.API.Infrastructure.Seed;
using AssetManagement.Inventory.API.Services.Auth.Implementations;
using AssetManagement.Inventory.API.Services.Auth.Interfaces;
using AssetManagement.Inventory.API.Services.Email.Implementations;
using AssetManagement.Inventory.API.Services.Email.Interfaces;
using AssetManagement.Inventory.API.Services.Implementations;
using AssetManagement.Inventory.API.Services.Interfaces;
using AssetManagement.Inventory.API.Validators.Environment;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// SERVICES
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateEnvironmentValidator>();
builder.Services.AddScoped<IEnvironmentService, EnvironmentService>();

builder.Environment.ContentRootPath = Directory.GetCurrentDirectory();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


// DATABASE
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// IDENTITY
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

// JWT AUTHENTICATION
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

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

        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
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
builder.Services.AddControllers();

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

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization(); 

app.MapControllers();
app.UseStaticFiles();
app.Run();
