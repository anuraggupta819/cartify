using ECommerce.Catalog.Domain.Entities;

namespace ECommerce.Catalog.Application.Abstractions;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    void Remove(Product product);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}
