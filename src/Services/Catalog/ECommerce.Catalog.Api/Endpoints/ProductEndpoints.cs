using ECommerce.Catalog.Application.Dtos;
using ECommerce.Catalog.Application.Services;

namespace ECommerce.Catalog.Api.Endpoints;

public static class ProductEndpoints
{
    public static RouteGroupBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", async (ProductService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetAllAsync(cancellationToken)));

        group.MapGet("/{id:guid}", async (Guid id, ProductService service, CancellationToken cancellationToken) =>
        {
            var product = await service.GetByIdAsync(id, cancellationToken);
            return product is null ? Results.NotFound() : Results.Ok(product);
        });

        group.MapPost("/", async (CreateProductRequest request, ProductService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var product = await service.CreateAsync(request, cancellationToken);
                return Results.Created($"/api/products/{product.Id}", product);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateProductRequest request, ProductService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var product = await service.UpdateAsync(id, request, cancellationToken);
                return product is null ? Results.NotFound() : Results.Ok(product);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        group.MapDelete("/{id:guid}", async (Guid id, ProductService service, CancellationToken cancellationToken) =>
        {
            var deleted = await service.DeleteAsync(id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        return group;
    }
}
