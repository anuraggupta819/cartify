using System.Net;
using System.Net.Http.Json;
using ECommerce.OrderManagement.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace ECommerce.OrderManagement.Infrastructure.ExternalServices;

public class HttpProductCatalogClient(HttpClient httpClient, IConfiguration configuration) : IProductCatalogClient
{
    public async Task<ProductSnapshot?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var baseUrl = configuration["Services:ProductCatalogBaseUrl"]
            ?? throw new InvalidOperationException("Configuration 'Services:ProductCatalogBaseUrl' is not configured.");

        var response = await httpClient.GetAsync($"{baseUrl}/api/products/{productId}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var product = await response.Content.ReadFromJsonAsync<ProductCatalogResponse>(cancellationToken: cancellationToken);
        return product is null ? null : new ProductSnapshot(product.Id, product.Name, product.Price);
    }

    private record ProductCatalogResponse(Guid Id, string Name, decimal Price);
}
