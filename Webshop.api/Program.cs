using dotenv.net;
using Microsoft.EntityFrameworkCore;
using Webshop.api.Endpoints;
using Webshop.api.Models;
using Webshop.api.Services;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// DB connection
var config = builder.Configuration;
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        $"Host={config["DB_HOST"]};" +
        $"Port={config["DB_PORT"] ?? "5432"};" +
        $"Database={config["DB_NAME"]};" +
        $"Username={config["DB_USER"]};" +
        $"Password={config["DB_PASSWORD"]};"
    ));

builder.Services.AddScoped<AuthService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapAuthEndpoints();
app.MapProductsEndpoints();
app.MapCartEndpoints();
app.MapOrdersEndpoints();

app.Run();
