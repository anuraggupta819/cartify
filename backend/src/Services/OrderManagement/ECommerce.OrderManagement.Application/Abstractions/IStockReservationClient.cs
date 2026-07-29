namespace ECommerce.OrderManagement.Application.Abstractions;

public interface IStockReservationClient
{
    Task<bool> ReserveAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);

    Task ReleaseAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);
}
