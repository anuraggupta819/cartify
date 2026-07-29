using System.Security.Claims;
using ECommerce.OrderManagement.Api.Security;
using ECommerce.OrderManagement.Application.Dtos;
using ECommerce.OrderManagement.Application.Services;

namespace ECommerce.OrderManagement.Api.Endpoints;

public static class OrderEndpoints
{
    public static RouteGroupBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").WithTags("Orders").RequireAuthorization();

        group.MapPost("/", async (CreateOrderRequest request, ClaimsPrincipal user, OrderService service, CancellationToken cancellationToken) =>
        {
            var order = await service.CheckoutAsync(user.GetUserId(), request, cancellationToken);
            return Results.Created($"/api/orders/{order.Id}", order);
        });

        group.MapGet("/mine", async (ClaimsPrincipal user, OrderService service, CancellationToken cancellationToken, int pageNumber = 1, int pageSize = 20) =>
            Results.Ok(await service.GetMineAsync(user.GetUserId(), pageNumber, pageSize, cancellationToken)));

        group.MapGet("/{id:guid}", async (Guid id, ClaimsPrincipal user, OrderService service, CancellationToken cancellationToken) =>
        {
            var order = await service.GetByIdForUserAsync(id, user.GetUserId(), cancellationToken);
            return order is null ? Results.NotFound() : Results.Ok(order);
        });

        group.MapPost("/{id:guid}/confirm-payment", async (Guid id, OrderService service, CancellationToken cancellationToken) =>
        {
            var confirmed = await service.ConfirmPaymentAsync(id, cancellationToken);
            return confirmed ? Results.NoContent() : Results.NotFound();
        });

        group.MapPost("/{id:guid}/cancel", async (Guid id, OrderService service, CancellationToken cancellationToken) =>
        {
            var cancelled = await service.CancelAsync(id, cancellationToken);
            return cancelled ? Results.NoContent() : Results.NotFound();
        });

        return group;
    }
}
