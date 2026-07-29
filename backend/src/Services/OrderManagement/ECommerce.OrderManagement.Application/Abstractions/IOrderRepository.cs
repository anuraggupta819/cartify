using ECommerce.OrderManagement.Domain.Entities;

namespace ECommerce.OrderManagement.Application.Abstractions;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Order> Items, int TotalCount)> GetPagedForUserAsync(
        Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Order>> GetExpiredPendingAsync(DateTime olderThanUtc, CancellationToken cancellationToken = default);

    Task AddAsync(Order order, CancellationToken cancellationToken = default);
}
