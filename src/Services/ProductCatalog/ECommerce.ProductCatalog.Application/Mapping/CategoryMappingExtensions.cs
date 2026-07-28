using ECommerce.ProductCatalog.Application.Dtos;
using ECommerce.ProductCatalog.Domain.Entities;

namespace ECommerce.ProductCatalog.Application.Mapping;

public static class CategoryMappingExtensions
{
    public static CategoryDto ToDto(this Category category) => new(category.Id, category.Name);
}
