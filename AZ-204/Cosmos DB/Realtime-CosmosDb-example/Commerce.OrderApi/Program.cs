using Commerce.OrderApi.Config;
using Commerce.OrderApi.Models;
using Commerce.OrderApi.Repositories;
using Commerce.OrderApi.Services;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Bind config
builder.Services.Configure<CosmosDbConfig>(
    builder.Configuration.GetSection("CosmosDb"));

// CosmosClient
builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<
        Microsoft.Extensions.Options.IOptions<CosmosDbConfig>>().Value;

    return new CosmosClient(config.AccountEndpoint, config.AccountKey);
});

// Repositories
builder.Services.AddSingleton<ICosmosRepository<Order>>(sp =>
{
    var config = sp.GetRequiredService<
        Microsoft.Extensions.Options.IOptions<CosmosDbConfig>>().Value;
    var client = sp.GetRequiredService<CosmosClient>();
    return new CosmosRepository<Order>(client, config.DatabaseId, config.OrdersContainerId);
});

builder.Services.AddSingleton<ICosmosRepository<InventoryItem>>(sp =>
{
    var config = sp.GetRequiredService<
        Microsoft.Extensions.Options.IOptions<CosmosDbConfig>>().Value;
    var client = sp.GetRequiredService<CosmosClient>();
    return new CosmosRepository<InventoryItem>(client, config.DatabaseId, config.InventoryContainerId);
});

// OrderService
builder.Services.AddScoped<OrderService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
