using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ECommerce.PaymentProcessing.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ECommerce.PaymentProcessing.Infrastructure.ExternalServices;

public class HttpOrderQueryClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : IOrderQueryClient
{
    public async Task<OrderSummary?> GetOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var baseUrl = configuration["Services:OrderManagementBaseUrl"]
            ?? throw new InvalidOperationException("Configuration 'Services:OrderManagementBaseUrl' is not configured.");

        using var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/api/orders/{orderId}");
        var token = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(token);
        }

        var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var order = await response.Content.ReadFromJsonAsync<OrderResponse>(cancellationToken: cancellationToken);
        return order is null ? null : new OrderSummary(order.Id, order.TotalAmount, order.Status);
    }

    private record OrderResponse(Guid Id, string Status, decimal TotalAmount, DateTime CreatedAtUtc);
}
