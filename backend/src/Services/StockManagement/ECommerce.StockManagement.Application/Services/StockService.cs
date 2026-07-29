using ECommerce.StockManagement.Application.Abstractions;
using ECommerce.StockManagement.Application.Dtos;
using ECommerce.StockManagement.Application.Mapping;

namespace ECommerce.StockManagement.Application.Services;

public class StockService(IStockRepository stockRepository)
{
    public async Task<StockDto?> GetAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var stock = await stockRepository.GetByProductIdAsync(productId, cancellationToken);
        return stock?.ToDto();
    }

    public Task SetQuantityAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        if (quantity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity cannot be negative.");
        }

        return stockRepository.UpsertQuantityAsync(productId, quantity, cancellationToken);
    }

    public Task<bool> ReserveAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");
        }

        return stockRepository.TryReserveAsync(productId, quantity, cancellationToken);
    }

    public Task ReleaseAsync(Guid productId, int quantity, CancellationToken cancellationToken = default) =>
        stockRepository.ReleaseAsync(productId, quantity, cancellationToken);

    public Task FinalizeAsync(Guid productId, int quantity, CancellationToken cancellationToken = default) =>
        stockRepository.FinalizeAsync(productId, quantity, cancellationToken);
}
