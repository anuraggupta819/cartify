using System.Net.Http.Headers;
using ECommerce.PaymentProcessing.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ECommerce.PaymentProcessing.Infrastructure.ExternalServices;

public class HttpOrderPaymentNotifier(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : IOrderPaymentNotifier
{
    public Task NotifyPaymentSucceededAsync(Guid orderId, CancellationToken cancellationToken = default) =>
        PostAsync($"/api/orders/{orderId}/confirm-payment", cancellationToken);

    public Task NotifyPaymentFailedAsync(Guid orderId, CancellationToken cancellationToken = default) =>
        PostAsync($"/api/orders/{orderId}/cancel", cancellationToken);

    private async Task PostAsync(string path, CancellationToken cancellationToken)
    {
        var baseUrl = configuration["Services:OrderManagementBaseUrl"]
            ?? throw new InvalidOperationException("Configuration 'Services:OrderManagementBaseUrl' is not configured.");

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}{path}");
        var token = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(token);
        }

        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
