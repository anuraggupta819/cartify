namespace ECommerce.StockManagement.Domain.Entities;

public class Stock
{
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public int Reserved { get; private set; }

    public int Available => Quantity - Reserved;

    private Stock() { }

    public Stock(Guid productId, int quantity)
    {
        if (quantity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity cannot be negative.");
        }

        ProductId = productId;
        Quantity = quantity;
        Reserved = 0;
    }
}
