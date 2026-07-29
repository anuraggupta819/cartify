namespace ECommerce.OrderManagement.Application.Abstractions;

public record ProductSnapshot(Guid ProductId, string Name, decimal Price);

public interface IProductCatalogClient
{
    Task<ProductSnapshot?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default);
}
