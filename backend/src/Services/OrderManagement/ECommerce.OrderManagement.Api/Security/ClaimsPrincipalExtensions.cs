using System.Security.Claims;

namespace ECommerce.OrderManagement.Api.Security;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var sub = user.FindFirst("sub")?.Value;
        if (sub is null || !Guid.TryParse(sub, out var userId))
        {
            throw new InvalidOperationException("Authenticated user has no valid 'sub' claim.");
        }

        return userId;
    }
}
