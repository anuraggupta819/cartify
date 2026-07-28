namespace ECommerce.Catalog.Application.Dtos;

public record CategoryDto(Guid Id, string Name);

public record CreateCategoryRequest(string Name);
