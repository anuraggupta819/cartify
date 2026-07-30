namespace ECommerce.OrderManagement.Application.Exceptions;

public class InsufficientStockException(Guid productId, string productName, int available)
    : Exception(available <= 0
        ? $"\"{productName}\" is out of stock."
        : $"Only {available} unit(s) of \"{productName}\" left in stock.")
{
    public Guid ProductId { get; } = productId;
    public string ProductName { get; } = productName;
    public int Available { get; } = available;
}
