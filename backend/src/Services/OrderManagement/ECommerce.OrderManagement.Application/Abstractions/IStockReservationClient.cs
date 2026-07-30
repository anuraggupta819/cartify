namespace ECommerce.OrderManagement.Application.Abstractions;

public record StockReservationResult(bool Success, int Available);

public interface IStockReservationClient
{
    Task<StockReservationResult> ReserveAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);

    Task ReleaseAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);
}
