namespace ECommerce.Catalog.Application.Dtos;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    string Sku,
    decimal Price,
    Guid CategoryId,
    DateTime CreatedAtUtc);

public record CreateProductRequest(
    string Name,
    string Description,
    string Sku,
    decimal Price,
    Guid CategoryId);

public record UpdateProductRequest(
    string Name,
    string Description,
    decimal Price,
    Guid CategoryId);
