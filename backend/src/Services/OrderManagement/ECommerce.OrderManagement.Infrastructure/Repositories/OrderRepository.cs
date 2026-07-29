using ECommerce.OrderManagement.Application.Abstractions;
using ECommerce.OrderManagement.Domain.Entities;
using ECommerce.OrderManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.OrderManagement.Infrastructure.Repositories;

public class OrderRepository(OrderManagementDbContext dbContext) : IOrderRepository
{
    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Orders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<Order> Items, int TotalCount)> GetPagedForUserAsync(
        Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Orders.Where(o => o.UserId == userId).OrderByDescending(o => o.CreatedAtUtc);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Include(o => o.Lines)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<Order>> GetExpiredPendingAsync(DateTime olderThanUtc, CancellationToken cancellationToken = default) =>
        await dbContext.Orders
            .Include(o => o.Lines)
            .Where(o => o.Status == OrderStatus.PendingPayment && o.CreatedAtUtc < olderThanUtc)
            .ToListAsync(cancellationToken);

    public Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        dbContext.Orders.Add(order);
        return Task.CompletedTask;
    }
}
