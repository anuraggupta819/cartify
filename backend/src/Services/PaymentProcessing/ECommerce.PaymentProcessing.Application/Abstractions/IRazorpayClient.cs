namespace ECommerce.PaymentProcessing.Application.Abstractions;

public record RazorpayOrderResult(string RazorpayOrderId, long AmountInPaise, string Currency);

public interface IRazorpayClient
{
    Task<RazorpayOrderResult> CreateOrderAsync(long amountInPaise, string currency, string receipt, CancellationToken cancellationToken = default);

    bool VerifySignature(string razorpayOrderId, string razorpayPaymentId, string razorpaySignature);
}
