using ECommerce.Catalog.Domain.Entities;

namespace ECommerce.Catalog.UnitTests;

public class ProductTests
{
    [Fact]
    public void Constructor_WithValidArguments_CreatesProduct()
    {
        var categoryId = Guid.NewGuid();

        var product = new Product(Guid.NewGuid(), "Widget", "A useful widget", "SKU-1", 9.99m, categoryId);

        Assert.Equal("Widget", product.Name);
        Assert.Equal("SKU-1", product.Sku);
        Assert.Equal(9.99m, product.Price);
        Assert.Equal(categoryId, product.CategoryId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithNonPositivePrice_Throws(decimal price)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Product(Guid.NewGuid(), "Widget", "desc", "SKU-1", price, Guid.NewGuid()));
    }

    [Fact]
    public void Constructor_WithEmptyName_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new Product(Guid.NewGuid(), "", "desc", "SKU-1", 9.99m, Guid.NewGuid()));
    }

    [Fact]
    public void Constructor_WithEmptySku_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new Product(Guid.NewGuid(), "Widget", "desc", "", 9.99m, Guid.NewGuid()));
    }

    [Fact]
    public void UpdateDetails_WithValidArguments_UpdatesFields()
    {
        var product = new Product(Guid.NewGuid(), "Widget", "desc", "SKU-1", 9.99m, Guid.NewGuid());
        var newCategoryId = Guid.NewGuid();

        product.UpdateDetails("New Widget", "new desc", 19.99m, newCategoryId);

        Assert.Equal("New Widget", product.Name);
        Assert.Equal(19.99m, product.Price);
        Assert.Equal(newCategoryId, product.CategoryId);
    }

    [Fact]
    public void UpdateDetails_WithNonPositivePrice_Throws()
    {
        var product = new Product(Guid.NewGuid(), "Widget", "desc", "SKU-1", 9.99m, Guid.NewGuid());

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            product.UpdateDetails("Widget", "desc", 0m, Guid.NewGuid()));
    }
}
