using ECommerce.Catalog.Application.Abstractions;
using ECommerce.Catalog.Application.Dtos;
using ECommerce.Catalog.Domain.Entities;

namespace ECommerce.Catalog.Application.Services;

public class CategoryService(ICategoryRepository categoryRepository)
{
    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await categoryRepository.GetAllAsync(cancellationToken);
        return categories.Select(ToDto).ToList();
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = new Category(Guid.NewGuid(), request.Name);
        await categoryRepository.AddAsync(category, cancellationToken);
        await categoryRepository.SaveChangesAsync(cancellationToken);
        return ToDto(category);
    }

    private static CategoryDto ToDto(Category category) => new(category.Id, category.Name);
}
