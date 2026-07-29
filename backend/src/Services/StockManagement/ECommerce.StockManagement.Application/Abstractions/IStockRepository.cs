using ECommerce.StockManagement.Domain.Entities;

namespace ECommerce.StockManagement.Application.Abstractions;

public interface IStockRepository
{
    Task<Stock?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

    Task UpsertQuantityAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);

    Task<bool> TryReserveAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);

    Task ReleaseAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);

    Task FinalizeAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);
}
