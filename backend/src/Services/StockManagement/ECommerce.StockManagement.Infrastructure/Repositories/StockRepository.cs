using ECommerce.StockManagement.Application.Abstractions;
using ECommerce.StockManagement.Domain.Entities;
using ECommerce.StockManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.StockManagement.Infrastructure.Repositories;

public class StockRepository(StockManagementDbContext dbContext) : IStockRepository
{
    public Task<Stock?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default) =>
        dbContext.Stocks.AsNoTracking().FirstOrDefaultAsync(s => s.ProductId == productId, cancellationToken);

    public async Task UpsertQuantityAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        var updated = await dbContext.Stocks
            .Where(s => s.ProductId == productId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.Quantity, quantity), cancellationToken);

        if (updated == 0)
        {
            dbContext.Stocks.Add(new Stock(productId, quantity));
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    // Atomic conditional update (not load-then-save) so concurrent checkouts can't both
    // succeed in reserving the same last unit of stock.
    public async Task<bool> TryReserveAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        var updated = await dbContext.Stocks
            .Where(s => s.ProductId == productId && s.Quantity - s.Reserved >= quantity)
            .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.Reserved, s => s.Reserved + quantity), cancellationToken);

        return updated == 1;
    }

    public Task ReleaseAsync(Guid productId, int quantity, CancellationToken cancellationToken = default) =>
        dbContext.Stocks
            .Where(s => s.ProductId == productId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(
                s => s.Reserved,
                s => s.Reserved - quantity < 0 ? 0 : s.Reserved - quantity), cancellationToken);

    public Task FinalizeAsync(Guid productId, int quantity, CancellationToken cancellationToken = default) =>
        dbContext.Stocks
            .Where(s => s.ProductId == productId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(s => s.Quantity, s => s.Quantity - quantity < 0 ? 0 : s.Quantity - quantity)
                .SetProperty(s => s.Reserved, s => s.Reserved - quantity < 0 ? 0 : s.Reserved - quantity), cancellationToken);
}
