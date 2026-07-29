namespace ECommerce.ProductCatalog.Application.Abstractions;

public interface IStockProvisioningClient
{
    Task ProvisionAsync(Guid productId, int initialQuantity, CancellationToken cancellationToken = default);
}
