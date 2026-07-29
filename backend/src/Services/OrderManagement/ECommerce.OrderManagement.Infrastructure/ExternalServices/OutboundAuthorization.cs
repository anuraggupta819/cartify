using ECommerce.OrderManagement.Infrastructure.Security;
using Microsoft.AspNetCore.Http;

namespace ECommerce.OrderManagement.Infrastructure.ExternalServices;

// Forwards the current request's bearer token to downstream services when one is present
// (normal checkout/order calls); falls back to a self-minted system token when there's no
// ambient HTTP request (the background sweep calling out on its own).
public class OutboundAuthorization(IHttpContextAccessor httpContextAccessor, SystemJwtIssuer systemJwtIssuer)
{
    public string Resolve()
    {
        var forwarded = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        return !string.IsNullOrEmpty(forwarded) ? forwarded : $"Bearer {systemJwtIssuer.IssueToken()}";
    }
}
