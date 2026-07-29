using ECommerce.ProductCatalog.Application.Abstractions;

namespace ECommerce.ProductCatalog.IntegrationTests;

// StockManagement isn't part of this test host — swap the real HTTP client for a no-op
// so ProductCatalog's own endpoint tests don't depend on another service being reachable.
public class FakeStockProvisioningClient : IStockProvisioningClient
{
    public Task ProvisionAsync(Guid productId, int initialQuantity, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
