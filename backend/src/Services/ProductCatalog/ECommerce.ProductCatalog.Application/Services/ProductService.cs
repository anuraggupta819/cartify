using ECommerce.ProductCatalog.Application.Abstractions;
using ECommerce.ProductCatalog.Application.Common;
using ECommerce.ProductCatalog.Application.Dtos;
using ECommerce.ProductCatalog.Application.Mapping;
using ECommerce.ProductCatalog.Domain.Entities;

namespace ECommerce.ProductCatalog.Application.Services;

public class ProductService(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IStockProvisioningClient stockProvisioningClient)
{
    public async Task<PagedResult<ProductDto>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (items, totalCount) = await productRepository.GetPagedAsync(pageNumber, pageSize, cancellationToken);
        return new PagedResult<ProductDto>(items.Select(p => p.ToDto()).ToList(), totalCount, pageNumber, pageSize);
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);
        return product?.ToDto();
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        var categoryExists = await categoryRepository.ExistsAsync(request.CategoryId, cancellationToken);
        if (!categoryExists)
        {
            throw new ArgumentException($"Category '{request.CategoryId}' does not exist.", nameof(request.CategoryId));
        }

        var product = new Product(Guid.NewGuid(), request.Name, request.Description, request.Sku, request.Price, request.CategoryId, request.ImageUrl);
        await productRepository.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await stockProvisioningClient.ProvisionAsync(product.Id, request.InitialStockQuantity, cancellationToken);

        return product.ToDto();
    }

    public async Task<ProductDto?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return null;
        }

        var categoryExists = await categoryRepository.ExistsAsync(request.CategoryId, cancellationToken);
        if (!categoryExists)
        {
            throw new ArgumentException($"Category '{request.CategoryId}' does not exist.", nameof(request.CategoryId));
        }

        product.UpdateDetails(request.Name, request.Description, request.Price, request.CategoryId, request.ImageUrl);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return product.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return false;
        }

        productRepository.Remove(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
