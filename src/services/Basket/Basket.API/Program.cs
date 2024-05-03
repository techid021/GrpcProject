using Basket.API.Infrastructure.Repositories;
using Basket.API.Model;
using Basket.API.Services;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ProductApi;
using StackExchange.Redis;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddScoped<IProductService, ProductService>()
    .AddGrpcClient<ProductGrpc.ProductGrpcClient>((services, options) =>
    {

        options.Address = new Uri("https://localhost:7087");
    }).ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        return handler;
    });


builder.Services.AddSingleton(sp =>
{
    string connectionString = "127.0.0.1:6379";

    ConfigurationOptions options = new ConfigurationOptions
    {
        AbortOnConnectFail = false,
        EndPoints = { connectionString }
    };

    return ConnectionMultiplexer.Connect(options);
});

builder.Services.AddTransient<IBasketRepository, RedisBasketRepository>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
