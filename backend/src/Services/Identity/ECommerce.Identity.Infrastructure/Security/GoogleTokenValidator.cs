using ECommerce.Identity.Application.Abstractions;
using ECommerce.Identity.Application.Exceptions;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Identity.Infrastructure.Security;

public class GoogleTokenValidator(IConfiguration configuration) : IGoogleTokenValidator
{
    public async Task<GoogleUserInfo> ValidateAsync(string idToken, CancellationToken cancellationToken = default)
    {
        var clientId = configuration["Google:ClientId"]
            ?? throw new InvalidOperationException("Configuration 'Google:ClientId' is not set.");

        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [clientId],
            });

            return new GoogleUserInfo(payload.Subject, payload.Email, payload.Name);
        }
        catch (InvalidJwtException ex)
        {
            throw new AuthenticationFailedException("Invalid Google ID token.", ex);
        }
    }
}
