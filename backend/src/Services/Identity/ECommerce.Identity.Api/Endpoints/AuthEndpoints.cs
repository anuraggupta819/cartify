using ECommerce.Identity.Application.Dtos;
using ECommerce.Identity.Application.Services;

namespace ECommerce.Identity.Api.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/google", async (GoogleLoginRequest request, AuthService authService, CancellationToken cancellationToken) =>
            Results.Ok(await authService.GoogleLoginAsync(request, cancellationToken)));

        group.MapPost("/admin-login", async (AdminLoginRequest request, AuthService authService, CancellationToken cancellationToken) =>
            Results.Ok(await authService.AdminLoginAsync(request, cancellationToken)));

        return group;
    }
}
