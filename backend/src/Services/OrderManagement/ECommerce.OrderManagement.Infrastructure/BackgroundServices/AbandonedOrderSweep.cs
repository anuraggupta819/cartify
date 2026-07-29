using ECommerce.OrderManagement.Application.Abstractions;
using ECommerce.OrderManagement.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ECommerce.OrderManagement.Infrastructure.BackgroundServices;

public class AbandonedOrderSweep(IServiceScopeFactory scopeFactory, ILogger<AbandonedOrderSweep> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan ExpiryThreshold = TimeSpan.FromMinutes(30);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                var orderService = scope.ServiceProvider.GetRequiredService<OrderService>();

                var expired = await orderRepository.GetExpiredPendingAsync(DateTime.UtcNow - ExpiryThreshold, stoppingToken);
                foreach (var order in expired)
                {
                    await orderService.CancelAsync(order.Id, stoppingToken);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Abandoned order sweep failed.");
            }

            try
            {
                await Task.Delay(Interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // shutting down
            }
        }
    }
}
