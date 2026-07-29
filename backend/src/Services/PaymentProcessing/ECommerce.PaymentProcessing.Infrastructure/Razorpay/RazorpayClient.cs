using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using ECommerce.PaymentProcessing.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace ECommerce.PaymentProcessing.Infrastructure.Razorpay;

// Thin typed client over Razorpay's Orders REST API — deliberately not using a third-party
// SDK, since the two calls this project needs (create order, verify signature) are simple
// enough that a small auditable client is preferable to an unofficial NuGet dependency.
public class RazorpayClient(HttpClient httpClient, IConfiguration configuration) : IRazorpayClient
{
    public async Task<RazorpayOrderResult> CreateOrderAsync(long amountInPaise, string currency, string receipt, CancellationToken cancellationToken = default)
    {
        var (keyId, keySecret) = GetCredentials();

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.razorpay.com/v1/orders")
        {
            Content = JsonContent.Create(new { amount = amountInPaise, currency, receipt })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{keyId}:{keySecret}")));

        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<RazorpayOrderApiResponse>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Razorpay returned an empty response.");

        return new RazorpayOrderResult(body.Id, amountInPaise, currency);
    }

    public bool VerifySignature(string razorpayOrderId, string razorpayPaymentId, string razorpaySignature)
    {
        var (_, keySecret) = GetCredentials();

        var payload = $"{razorpayOrderId}|{razorpayPaymentId}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(keySecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var computedSignature = Convert.ToHexStringLower(hash);

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computedSignature),
            Encoding.UTF8.GetBytes(razorpaySignature));
    }

    private (string KeyId, string KeySecret) GetCredentials()
    {
        var keyId = configuration["Razorpay:KeyId"]
            ?? throw new InvalidOperationException("Configuration 'Razorpay:KeyId' is not set.");
        var keySecret = configuration["Razorpay:KeySecret"]
            ?? throw new InvalidOperationException("Configuration 'Razorpay:KeySecret' is not set.");

        return (keyId, keySecret);
    }

    private record RazorpayOrderApiResponse(string Id);
}
