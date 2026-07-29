using ECommerce.PaymentProcessing.Application.Abstractions;

namespace ECommerce.PaymentProcessing.Infrastructure.Persistence;

public class UnitOfWork(PaymentProcessingDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
