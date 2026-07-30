using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ECommerce.OrderManagement.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace ECommerce.OrderManagement.Infrastructure.ExternalServices;

public class HttpStockReservationClient(HttpClient httpClient, OutboundAuthorization outboundAuthorization, IConfiguration configuration) : IStockReservationClient
{
    public async Task<StockReservationResult> ReserveAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        using var request = BuildRequest($"/api/stock/{productId}/reserve", quantity);
        var response = await httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            var body = await response.Content.ReadFromJsonAsync<ConflictResponse>(cancellationToken: cancellationToken);
            return new StockReservationResult(false, body?.Available ?? 0);
        }

        response.EnsureSuccessStatusCode();
        return new StockReservationResult(true, 0);
    }

    private record ConflictResponse(string Message, int Available);

    public async Task ReleaseAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        using var request = BuildRequest($"/api/stock/{productId}/release", quantity);
        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private HttpRequestMessage BuildRequest(string path, int quantity)
    {
        var baseUrl = configuration["Services:StockManagementBaseUrl"]
            ?? throw new InvalidOperationException("Configuration 'Services:StockManagementBaseUrl' is not configured.");

        var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}{path}")
        {
            Content = JsonContent.Create(new { Quantity = quantity })
        };
        request.Headers.Authorization = AuthenticationHeaderValue.Parse(outboundAuthorization.Resolve());

        return request;
    }
}
