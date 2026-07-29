namespace ECommerce.ProductCatalog.Application.Dtos;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    string Sku,
    decimal Price,
    Guid CategoryId,
    string? ImageUrl,
    DateTime CreatedAtUtc);

public record CreateProductRequest(
    string Name,
    string Description,
    string Sku,
    decimal Price,
    Guid CategoryId,
    string? ImageUrl,
    int InitialStockQuantity);

public record UpdateProductRequest(
    string Name,
    string Description,
    decimal Price,
    Guid CategoryId,
    string? ImageUrl);
