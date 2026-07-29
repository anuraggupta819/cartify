namespace ECommerce.OrderManagement.Application.Exceptions;

public class InsufficientStockException(Guid productId) : Exception($"Insufficient stock for product '{productId}'.")
{
    public Guid ProductId { get; } = productId;
}
