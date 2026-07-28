namespace ECommerce.ProductCatalog.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Sku { get; private set; } = null!;
    public decimal Price { get; private set; }
    public Guid CategoryId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Product() { }

    public Product(Guid id, string name, string description, string sku, decimal price, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Product name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new ArgumentException("Product SKU is required.", nameof(sku));
        }

        if (price <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "Product price must be greater than zero.");
        }

        Id = id;
        Name = name;
        Description = description ?? string.Empty;
        Sku = sku;
        Price = price;
        CategoryId = categoryId;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateDetails(string name, string description, decimal price, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Product name is required.", nameof(name));
        }

        if (price <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "Product price must be greater than zero.");
        }

        Name = name;
        Description = description ?? string.Empty;
        Price = price;
        CategoryId = categoryId;
    }
}
