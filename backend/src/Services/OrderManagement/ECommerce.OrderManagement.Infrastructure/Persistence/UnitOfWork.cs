using ECommerce.OrderManagement.Application.Abstractions;

namespace ECommerce.OrderManagement.Infrastructure.Persistence;

public class UnitOfWork(OrderManagementDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
