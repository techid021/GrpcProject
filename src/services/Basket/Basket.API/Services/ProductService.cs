using Basket.API.Model;

using Grpc.Core;
using Grpc.Net.Client;
using ProductApi;

namespace Basket.API.Services;

public class ProductService : IProductService
{
    private readonly ProductGrpc.ProductGrpcClient _client;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        ProductGrpc.ProductGrpcClient client,
        ILogger<ProductService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<ProductItem?> GetProductAsync(int productId)
    {
        GetProductRequest request = new() { Id = productId };

        try
        {
            //var handler = new HttpClientHandler();
            //handler.ServerCertificateCustomValidationCallback =
            //    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            //var channel = GrpcChannel.ForAddress("https://localhost:7087",
            //    new GrpcChannelOptions { HttpHandler = handler });
            //var _client = new ProductGrpc.ProductGrpcClient(channel);

            GetProductResponse response = await _client.GetProductAsync(request);

            return new ProductItem(
                response.Id,
                response.Name,
                response.Description,
                response.Price);
        }
        catch (RpcException e)
        {
            _logger.LogWarning(e, "ERROR - Parameters: {@parameters}", request);

            return null;
        }
    }
}