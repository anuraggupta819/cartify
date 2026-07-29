namespace ECommerce.OrderManagement.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private readonly List<OrderLine> _lines = [];
    public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();

    public decimal TotalAmount => _lines.Sum(l => l.LineTotal);

    private Order() { }

    public Order(Guid userId, IEnumerable<OrderLine> lines)
    {
        var lineList = lines.ToList();
        if (lineList.Count == 0)
        {
            throw new ArgumentException("Order must have at least one line.", nameof(lines));
        }

        Id = Guid.NewGuid();
        UserId = userId;
        Status = OrderStatus.PendingPayment;
        CreatedAtUtc = DateTime.UtcNow;
        _lines.AddRange(lineList);
    }

    public void MarkPaid()
    {
        if (Status != OrderStatus.PendingPayment)
        {
            throw new InvalidOperationException($"Cannot mark order as Paid from status '{Status}'.");
        }

        Status = OrderStatus.Paid;
    }

    public void Cancel()
    {
        if (Status != OrderStatus.PendingPayment)
        {
            throw new InvalidOperationException($"Cannot cancel order from status '{Status}'.");
        }

        Status = OrderStatus.Cancelled;
    }
}
