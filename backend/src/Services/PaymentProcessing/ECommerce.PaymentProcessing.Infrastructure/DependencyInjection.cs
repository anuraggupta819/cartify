using ECommerce.PaymentProcessing.Application.Abstractions;
using ECommerce.PaymentProcessing.Application.Services;
using ECommerce.PaymentProcessing.Infrastructure.ExternalServices;
using ECommerce.PaymentProcessing.Infrastructure.Persistence;
using ECommerce.PaymentProcessing.Infrastructure.Razorpay;
using ECommerce.PaymentProcessing.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.PaymentProcessing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPaymentProcessingInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PaymentProcessingDb")
            ?? throw new InvalidOperationException("Connection string 'PaymentProcessingDb' is not configured.");

        services.AddDbContext<PaymentProcessingDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<PaymentService>();

        services.AddHttpContextAccessor();
        services.AddHttpClient<IRazorpayClient, RazorpayClient>();
        services.AddHttpClient<IOrderQueryClient, HttpOrderQueryClient>();
        services.AddHttpClient<IOrderPaymentNotifier, HttpOrderPaymentNotifier>();

        return services;
    }
}
