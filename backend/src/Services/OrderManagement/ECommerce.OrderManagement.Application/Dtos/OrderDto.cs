namespace ECommerce.OrderManagement.Application.Dtos;

public record OrderLineDto(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity, decimal LineTotal);

public record OrderDto(Guid Id, string Status, decimal TotalAmount, DateTime CreatedAtUtc, IReadOnlyList<OrderLineDto> Lines);

public record CreateOrderLineRequest(Guid ProductId, int Quantity);

public record CreateOrderRequest(IReadOnlyList<CreateOrderLineRequest> Lines);
