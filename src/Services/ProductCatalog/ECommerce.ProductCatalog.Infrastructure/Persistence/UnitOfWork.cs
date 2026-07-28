using ECommerce.ProductCatalog.Application.Abstractions;

namespace ECommerce.ProductCatalog.Infrastructure.Persistence;

public class UnitOfWork(ProductCatalogDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
