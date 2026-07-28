using ECommerce.ProductCatalog.Application.Abstractions;
using ECommerce.ProductCatalog.Application.Dtos;
using ECommerce.ProductCatalog.Application.Mapping;
using ECommerce.ProductCatalog.Domain.Entities;

namespace ECommerce.ProductCatalog.Application.Services;

public class CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
{
    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await categoryRepository.GetAllAsync(cancellationToken);
        return categories.Select(c => c.ToDto()).ToList();
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = new Category(Guid.NewGuid(), request.Name);
        await categoryRepository.AddAsync(category, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return category.ToDto();
    }
}
