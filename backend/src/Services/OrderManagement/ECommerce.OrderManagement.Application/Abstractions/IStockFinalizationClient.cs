namespace ECommerce.OrderManagement.Application.Abstractions;

public interface IStockFinalizationClient
{
    Task FinalizeAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);
}
