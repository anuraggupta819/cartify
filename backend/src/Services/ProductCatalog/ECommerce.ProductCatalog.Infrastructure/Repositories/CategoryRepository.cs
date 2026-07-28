using ECommerce.ProductCatalog.Application.Abstractions;
using ECommerce.ProductCatalog.Domain.Entities;
using ECommerce.ProductCatalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.ProductCatalog.Infrastructure.Repositories;

public class CategoryRepository(ProductCatalogDbContext dbContext) : ICategoryRepository
{
    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Categories.AsNoTracking().ToListAsync(cancellationToken);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Categories.AnyAsync(c => c.Id == id, cancellationToken);

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default) =>
        await dbContext.Categories.AddAsync(category, cancellationToken);
}
