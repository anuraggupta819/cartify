namespace ECommerce.PaymentProcessing.Domain.Entities;

public class Payment
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public string RazorpayOrderId { get; private set; } = null!;
    public string? RazorpayPaymentId { get; private set; }
    public PaymentStatus Status { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Payment() { }

    public Payment(Guid orderId, string razorpayOrderId, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(razorpayOrderId))
        {
            throw new ArgumentException("Razorpay order id is required.", nameof(razorpayOrderId));
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        }

        Id = Guid.NewGuid();
        OrderId = orderId;
        RazorpayOrderId = razorpayOrderId;
        Amount = amount;
        Status = PaymentStatus.Created;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void MarkCaptured(string razorpayPaymentId)
    {
        if (Status != PaymentStatus.Created)
        {
            throw new InvalidOperationException($"Cannot mark payment as captured from status '{Status}'.");
        }

        RazorpayPaymentId = razorpayPaymentId;
        Status = PaymentStatus.Captured;
    }

    public void MarkFailed(string? razorpayPaymentId)
    {
        if (Status != PaymentStatus.Created)
        {
            throw new InvalidOperationException($"Cannot mark payment as failed from status '{Status}'.");
        }

        RazorpayPaymentId = razorpayPaymentId;
        Status = PaymentStatus.Failed;
    }
}
