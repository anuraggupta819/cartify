using ECommerce.Catalog.Application.Dtos;
using ECommerce.Catalog.Application.Services;

namespace ECommerce.Catalog.Api.Endpoints;

public static class CategoryEndpoints
{
    public static RouteGroupBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories").WithTags("Categories");

        group.MapGet("/", async (CategoryService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetAllAsync(cancellationToken)));

        group.MapPost("/", async (CreateCategoryRequest request, CategoryService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var category = await service.CreateAsync(request, cancellationToken);
                return Results.Created($"/api/categories/{category.Id}", category);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        return group;
    }
}
