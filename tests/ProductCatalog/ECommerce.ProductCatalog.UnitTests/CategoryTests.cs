using ECommerce.ProductCatalog.Domain.Entities;

namespace ECommerce.ProductCatalog.UnitTests;

public class CategoryTests
{
    [Fact]
    public void Constructor_WithEmptyName_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Category(Guid.NewGuid(), ""));
    }

    [Fact]
    public void Rename_WithEmptyName_Throws()
    {
        var category = new Category(Guid.NewGuid(), "Electronics");

        Assert.Throws<ArgumentException>(() => category.Rename(" "));
    }

    [Fact]
    public void Rename_WithValidName_UpdatesName()
    {
        var category = new Category(Guid.NewGuid(), "Electronics");

        category.Rename("Home Electronics");

        Assert.Equal("Home Electronics", category.Name);
    }
}
