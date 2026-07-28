using ECommerce.ProductCatalog.Application.Abstractions;
using ECommerce.ProductCatalog.Domain.Entities;
using ECommerce.ProductCatalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.ProductCatalog.Infrastructure.Repositories;

public class ProductRepository(ProductCatalogDbContext dbContext) : IProductRepository
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Products.AsNoTracking().OrderBy(p => p.CreatedAtUtc);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default) =>
        await dbContext.Products.AddAsync(product, cancellationToken);

    public void Remove(Product product) => dbContext.Products.Remove(product);
}
