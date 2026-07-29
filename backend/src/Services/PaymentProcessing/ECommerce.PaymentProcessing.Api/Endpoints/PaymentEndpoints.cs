using ECommerce.PaymentProcessing.Application.Dtos;
using ECommerce.PaymentProcessing.Application.Services;

namespace ECommerce.PaymentProcessing.Api.Endpoints;

public static class PaymentEndpoints
{
    public static RouteGroupBuilder MapPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payments").WithTags("Payments").RequireAuthorization();

        group.MapPost("/razorpay-order", async (CreateRazorpayOrderRequest request, PaymentService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.CreateRazorpayOrderAsync(request.OrderId, cancellationToken)));

        group.MapPost("/verify", async (VerifyPaymentRequest request, PaymentService service, CancellationToken cancellationToken) =>
        {
            var verified = await service.VerifyPaymentAsync(request, cancellationToken);
            return verified ? Results.Ok(new { verified = true }) : Results.BadRequest(new { verified = false });
        });

        return group;
    }
}
