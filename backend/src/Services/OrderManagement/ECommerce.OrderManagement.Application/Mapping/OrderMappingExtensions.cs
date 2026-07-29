using ECommerce.OrderManagement.Application.Dtos;
using ECommerce.OrderManagement.Domain.Entities;

namespace ECommerce.OrderManagement.Application.Mapping;

public static class OrderMappingExtensions
{
    public static OrderLineDto ToDto(this OrderLine line) =>
        new(line.ProductId, line.ProductName, line.UnitPrice, line.Quantity, line.LineTotal);

    public static OrderDto ToDto(this Order order) =>
        new(order.Id, order.Status.ToString(), order.TotalAmount, order.CreatedAtUtc, order.Lines.Select(l => l.ToDto()).ToList());
}
