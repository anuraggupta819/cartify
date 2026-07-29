namespace ECommerce.PaymentProcessing.Application.Abstractions;

public record OrderSummary(Guid OrderId, decimal TotalAmount, string Status);

public interface IOrderQueryClient
{
    Task<OrderSummary?> GetOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
}
