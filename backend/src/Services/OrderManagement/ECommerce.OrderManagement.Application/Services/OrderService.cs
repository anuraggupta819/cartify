using ECommerce.OrderManagement.Application.Abstractions;
using ECommerce.OrderManagement.Application.Common;
using ECommerce.OrderManagement.Application.Dtos;
using ECommerce.OrderManagement.Application.Exceptions;
using ECommerce.OrderManagement.Application.Mapping;
using ECommerce.OrderManagement.Domain.Entities;

namespace ECommerce.OrderManagement.Application.Services;

public class OrderService(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IProductCatalogClient productCatalogClient,
    IStockReservationClient stockReservationClient,
    IStockFinalizationClient stockFinalizationClient)
{
    public async Task<OrderDto> CheckoutAsync(Guid userId, CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Lines.Count == 0)
        {
            throw new ArgumentException("Order must contain at least one line.", nameof(request));
        }

        var lines = new List<OrderLine>();
        var reserved = new List<(Guid ProductId, int Quantity)>();

        try
        {
            foreach (var lineRequest in request.Lines)
            {
                var product = await productCatalogClient.GetProductAsync(lineRequest.ProductId, cancellationToken)
                    ?? throw new ArgumentException($"Product '{lineRequest.ProductId}' does not exist.", nameof(request));

                var reservation = await stockReservationClient.ReserveAsync(lineRequest.ProductId, lineRequest.Quantity, cancellationToken);
                if (!reservation.Success)
                {
                    throw new InsufficientStockException(lineRequest.ProductId, product.Name, reservation.Available);
                }

                reserved.Add((lineRequest.ProductId, lineRequest.Quantity));
                lines.Add(new OrderLine(product.ProductId, product.Name, product.Price, lineRequest.Quantity));
            }
        }
        catch
        {
            foreach (var (productId, quantity) in reserved)
            {
                await stockReservationClient.ReleaseAsync(productId, quantity, cancellationToken);
            }

            throw;
        }

        var order = new Order(userId, lines);
        await orderRepository.AddAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return order.ToDto();
    }

    public async Task<OrderDto?> GetByIdForUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdAsync(id, cancellationToken);
        return order is null || order.UserId != userId ? null : order.ToDto();
    }

    public async Task<PagedResult<OrderDto>> GetMineAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (items, totalCount) = await orderRepository.GetPagedForUserAsync(userId, pageNumber, pageSize, cancellationToken);
        return new PagedResult<OrderDto>(items.Select(o => o.ToDto()).ToList(), totalCount, pageNumber, pageSize);
    }

    public async Task<bool> ConfirmPaymentAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
        {
            return false;
        }

        order.MarkPaid();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (var line in order.Lines)
        {
            await stockFinalizationClient.FinalizeAsync(line.ProductId, line.Quantity, cancellationToken);
        }

        return true;
    }

    public async Task<bool> CancelAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null || order.Status != OrderStatus.PendingPayment)
        {
            return false;
        }

        order.Cancel();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (var line in order.Lines)
        {
            await stockReservationClient.ReleaseAsync(line.ProductId, line.Quantity, cancellationToken);
        }

        return true;
    }
}
