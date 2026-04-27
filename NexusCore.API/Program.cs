using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Webshop.api.Endpoints;
using Webshop.api.Models;
using Webshop.api.Services;

// Load enviroment variables from .env file
DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { "../.env" }));

// Disables legacy claim mapping to keep JWT names simple (e.g., 'role' instead of a URL)
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
var config = builder.Configuration;

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins(config["APP_URL"] ?? throw new InvalidOperationException("APP_URL configuration is missing."))
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

// JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["JWT_SECRET"] ?? 
                throw new InvalidOperationException("JWT_SECRET not configured!"))),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.NameIdentifier
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["access_token"];
                return Task.CompletedTask;
            }
        };
    });

// Roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("SuperAdmin"));
    options.AddPolicy("AdminLevel", policy => policy.RequireRole("SuperAdmin", "Admin"));
    options.AddPolicy("CustomerLevel", policy => policy.RequireRole("SuperAdmin", "Admin", "Customer"));
});

// DB connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        $"Host={config["DB_HOST"]};" +
        $"Port={config["DB_PORT"] ?? "5432"};" +
        $"Database={config["DB_NAME"]};" +
        $"Username={config["DB_USER"]};" +
        $"Password={config["DB_PASSWORD"]};"
    ));

// Fixes infinite loops in JSON by ignoring circular references (e.g., Order <-> OrderItem)
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddValidation();
builder.Services.AddHttpContextAccessor();

// Services
builder.Services.AddScoped<HelperService>();
builder.Services.AddScoped<MailService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("MyAllowSpecificOrigins");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Authentication
app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapAuthEndpoints();
app.MapProductsEndpoints();
app.MapCartEndpoints();
app.MapOrdersEndpoints();

app.Run();
