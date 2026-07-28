namespace ECommerce.ProductCatalog.Application.Dtos;

public record CategoryDto(Guid Id, string Name);

public record CreateCategoryRequest(string Name);
