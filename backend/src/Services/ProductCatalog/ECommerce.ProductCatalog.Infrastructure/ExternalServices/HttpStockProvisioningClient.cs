using System.Net.Http.Headers;
using System.Net.Http.Json;
using ECommerce.ProductCatalog.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ECommerce.ProductCatalog.Infrastructure.ExternalServices;

public class HttpStockProvisioningClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : IStockProvisioningClient
{
    public async Task ProvisionAsync(Guid productId, int initialQuantity, CancellationToken cancellationToken = default)
    {
        var baseUrl = configuration["Services:StockManagementBaseUrl"]
            ?? throw new InvalidOperationException("Configuration 'Services:StockManagementBaseUrl' is not configured.");

        using var request = new HttpRequestMessage(HttpMethod.Put, $"{baseUrl}/api/stock/{productId}")
        {
            Content = JsonContent.Create(new { Quantity = initialQuantity })
        };

        var token = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(token);
        }

        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
