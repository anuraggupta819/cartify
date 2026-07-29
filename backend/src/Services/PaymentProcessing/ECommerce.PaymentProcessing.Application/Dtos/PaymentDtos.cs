namespace ECommerce.PaymentProcessing.Application.Dtos;

public record CreateRazorpayOrderRequest(Guid OrderId);

public record RazorpayOrderResponseDto(string RazorpayOrderId, string RazorpayKeyId, long AmountInPaise, string Currency);

public record VerifyPaymentRequest(string RazorpayOrderId, string RazorpayPaymentId, string RazorpaySignature);
