using ECommerce.Catalog.Application.Abstractions;
using ECommerce.Catalog.Application.Dtos;
using ECommerce.Catalog.Domain.Entities;

namespace ECommerce.Catalog.Application.Services;

public class ProductService(IProductRepository productRepository)
{
    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await productRepository.GetAllAsync(cancellationToken);
        return products.Select(ToDto).ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);
        return product is null ? null : ToDto(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = new Product(Guid.NewGuid(), request.Name, request.Description, request.Sku, request.Price, request.CategoryId);
        await productRepository.AddAsync(product, cancellationToken);
        await productRepository.SaveChangesAsync(cancellationToken);
        return ToDto(product);
    }

    public async Task<ProductDto?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return null;
        }

        product.UpdateDetails(request.Name, request.Description, request.Price, request.CategoryId);
        await productRepository.SaveChangesAsync(cancellationToken);
        return ToDto(product);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return false;
        }

        productRepository.Remove(product);
        await productRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ProductDto ToDto(Product product) =>
        new(product.Id, product.Name, product.Description, product.Sku, product.Price, product.CategoryId, product.CreatedAtUtc);
}
