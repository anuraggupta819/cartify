using ECommerce.PaymentProcessing.Application.Abstractions;
using ECommerce.PaymentProcessing.Application.Dtos;
using ECommerce.PaymentProcessing.Application.Options;
using ECommerce.PaymentProcessing.Domain.Entities;

namespace ECommerce.PaymentProcessing.Application.Services;

public class PaymentService(
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork,
    IRazorpayClient razorpayClient,
    IOrderQueryClient orderQueryClient,
    IOrderPaymentNotifier orderPaymentNotifier,
    RazorpayPublicSettings razorpaySettings)
{
    private const string Currency = "INR";

    public async Task<RazorpayOrderResponseDto> CreateRazorpayOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await orderQueryClient.GetOrderAsync(orderId, cancellationToken)
            ?? throw new ArgumentException($"Order '{orderId}' does not exist.", nameof(orderId));

        if (order.Status != "PendingPayment")
        {
            throw new InvalidOperationException($"Order '{orderId}' is not awaiting payment (status: {order.Status}).");
        }

        var amountInPaise = (long)Math.Round(order.TotalAmount * 100, MidpointRounding.AwayFromZero);
        var razorpayOrder = await razorpayClient.CreateOrderAsync(amountInPaise, Currency, orderId.ToString(), cancellationToken);

        var payment = new Payment(orderId, razorpayOrder.RazorpayOrderId, order.TotalAmount);
        await paymentRepository.AddAsync(payment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new RazorpayOrderResponseDto(razorpayOrder.RazorpayOrderId, razorpaySettings.KeyId, amountInPaise, Currency);
    }

    public async Task<bool> VerifyPaymentAsync(VerifyPaymentRequest request, CancellationToken cancellationToken = default)
    {
        var payment = await paymentRepository.GetByRazorpayOrderIdAsync(request.RazorpayOrderId, cancellationToken)
            ?? throw new ArgumentException($"No payment found for Razorpay order '{request.RazorpayOrderId}'.", nameof(request));

        var isValid = razorpayClient.VerifySignature(request.RazorpayOrderId, request.RazorpayPaymentId, request.RazorpaySignature);

        if (isValid)
        {
            payment.MarkCaptured(request.RazorpayPaymentId);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await orderPaymentNotifier.NotifyPaymentSucceededAsync(payment.OrderId, cancellationToken);
            return true;
        }

        payment.MarkFailed(request.RazorpayPaymentId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await orderPaymentNotifier.NotifyPaymentFailedAsync(payment.OrderId, cancellationToken);
        return false;
    }
}
