using ECommerce.Catalog.Application.Abstractions;
using ECommerce.Catalog.Domain.Entities;
using ECommerce.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Catalog.Infrastructure.Repositories;

public class CategoryRepository(CatalogDbContext dbContext) : ICategoryRepository
{
    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Categories.AsNoTracking().ToListAsync(cancellationToken);

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default) =>
        await dbContext.Categories.AddAsync(category, cancellationToken);

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken) >= 0;
}
