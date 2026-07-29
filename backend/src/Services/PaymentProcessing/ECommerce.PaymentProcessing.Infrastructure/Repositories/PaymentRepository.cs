using ECommerce.PaymentProcessing.Application.Abstractions;
using ECommerce.PaymentProcessing.Domain.Entities;
using ECommerce.PaymentProcessing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.PaymentProcessing.Infrastructure.Repositories;

public class PaymentRepository(PaymentProcessingDbContext dbContext) : IPaymentRepository
{
    public Task<Payment?> GetByRazorpayOrderIdAsync(string razorpayOrderId, CancellationToken cancellationToken = default) =>
        dbContext.Payments.FirstOrDefaultAsync(p => p.RazorpayOrderId == razorpayOrderId, cancellationToken);

    public Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        dbContext.Payments.Add(payment);
        return Task.CompletedTask;
    }
}
