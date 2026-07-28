using ECommerce.ProductCatalog.Application.Dtos;
using ECommerce.ProductCatalog.Domain.Entities;

namespace ECommerce.ProductCatalog.Application.Mapping;

public static class ProductMappingExtensions
{
    public static ProductDto ToDto(this Product product) =>
        new(product.Id, product.Name, product.Description, product.Sku, product.Price, product.CategoryId, product.CreatedAtUtc);
}
