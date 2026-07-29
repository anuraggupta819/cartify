using System.Net.Http.Headers;
using System.Net.Http.Json;
using ECommerce.OrderManagement.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace ECommerce.OrderManagement.Infrastructure.ExternalServices;

public class HttpStockFinalizationClient(HttpClient httpClient, OutboundAuthorization outboundAuthorization, IConfiguration configuration) : IStockFinalizationClient
{
    public async Task FinalizeAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        var baseUrl = configuration["Services:StockManagementBaseUrl"]
            ?? throw new InvalidOperationException("Configuration 'Services:StockManagementBaseUrl' is not configured.");

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/api/stock/{productId}/finalize")
        {
            Content = JsonContent.Create(new { Quantity = quantity })
        };
        request.Headers.Authorization = AuthenticationHeaderValue.Parse(outboundAuthorization.Resolve());

        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
