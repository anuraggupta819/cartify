using ECommerce.PaymentProcessing.Domain.Entities;

namespace ECommerce.PaymentProcessing.Application.Abstractions;

public interface IPaymentRepository
{
    Task<Payment?> GetByRazorpayOrderIdAsync(string razorpayOrderId, CancellationToken cancellationToken = default);

    Task AddAsync(Payment payment, CancellationToken cancellationToken = default);
}
