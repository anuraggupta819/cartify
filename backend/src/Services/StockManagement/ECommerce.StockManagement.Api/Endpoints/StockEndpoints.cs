using ECommerce.StockManagement.Application.Dtos;
using ECommerce.StockManagement.Application.Services;

namespace ECommerce.StockManagement.Api.Endpoints;

public static class StockEndpoints
{
    public static RouteGroupBuilder MapStockEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/stock").WithTags("Stock");

        group.MapGet("/{productId:guid}", async (Guid productId, StockService service, CancellationToken cancellationToken) =>
        {
            var stock = await service.GetAsync(productId, cancellationToken);
            return stock is null ? Results.NotFound() : Results.Ok(stock);
        }).AllowAnonymous();

        group.MapPut("/{productId:guid}", async (Guid productId, SetStockQuantityRequest request, StockService service, CancellationToken cancellationToken) =>
        {
            await service.SetQuantityAsync(productId, request.Quantity, cancellationToken);
            return Results.NoContent();
        }).RequireAuthorization("RequireAdminRole");

        group.MapPost("/{productId:guid}/reserve", async (Guid productId, StockAdjustmentRequest request, StockService service, CancellationToken cancellationToken) =>
        {
            var reserved = await service.ReserveAsync(productId, request.Quantity, cancellationToken);
            return reserved ? Results.NoContent() : Results.Conflict(new { message = "Insufficient stock." });
        }).RequireAuthorization();

        group.MapPost("/{productId:guid}/release", async (Guid productId, StockAdjustmentRequest request, StockService service, CancellationToken cancellationToken) =>
        {
            await service.ReleaseAsync(productId, request.Quantity, cancellationToken);
            return Results.NoContent();
        }).RequireAuthorization();

        group.MapPost("/{productId:guid}/finalize", async (Guid productId, StockAdjustmentRequest request, StockService service, CancellationToken cancellationToken) =>
        {
            await service.FinalizeAsync(productId, request.Quantity, cancellationToken);
            return Results.NoContent();
        }).RequireAuthorization();

        return group;
    }
}
