using ECommerce.Identity.Application.Abstractions;

namespace ECommerce.Identity.Infrastructure.Persistence;

public class UnitOfWork(IdentityDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
