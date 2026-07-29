namespace ECommerce.Identity.Application.Abstractions;

public record GoogleUserInfo(string Sub, string Email, string? Name);

public interface IGoogleTokenValidator
{
    Task<GoogleUserInfo> ValidateAsync(string idToken, CancellationToken cancellationToken = default);
}
