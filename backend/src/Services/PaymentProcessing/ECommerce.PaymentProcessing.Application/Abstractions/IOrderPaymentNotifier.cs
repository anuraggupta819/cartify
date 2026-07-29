namespace ECommerce.PaymentProcessing.Application.Abstractions;

public interface IOrderPaymentNotifier
{
    Task NotifyPaymentSucceededAsync(Guid orderId, CancellationToken cancellationToken = default);

    Task NotifyPaymentFailedAsync(Guid orderId, CancellationToken cancellationToken = default);
}
