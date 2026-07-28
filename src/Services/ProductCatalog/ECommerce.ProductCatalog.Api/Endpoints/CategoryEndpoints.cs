using ECommerce.ProductCatalog.Application.Dtos;
using ECommerce.ProductCatalog.Application.Services;

namespace ECommerce.ProductCatalog.Api.Endpoints;

public static class CategoryEndpoints
{
    public static RouteGroupBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories").WithTags("Categories");

        group.MapGet("/", async (CategoryService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetAllAsync(cancellationToken)));

        group.MapPost("/", async (CreateCategoryRequest request, CategoryService service, CancellationToken cancellationToken) =>
        {
            var category = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/api/categories/{category.Id}", category);
        });

        return group;
    }
}
