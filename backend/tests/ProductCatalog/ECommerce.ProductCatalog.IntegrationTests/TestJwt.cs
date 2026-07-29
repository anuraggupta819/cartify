using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ECommerce.ProductCatalog.IntegrationTests;

public static class TestJwt
{
    // Matches appsettings.Development.json exactly: WebApplicationFactory defaults to the
    // Development environment, so that file's Jwt section is what's actually in effect during
    // tests - ConfigureAppConfiguration's in-memory override for these three keys does not
    // take precedence over it (confirmed via IDX10517 signature-mismatch diagnostics in CI).
    public const string SigningKey = "local-dev-only-signing-key-not-used-in-production-32chars+";
    public const string Issuer = "cartify-identity";
    public const string Audience = "cartify";

    public static string CreateAdminToken()
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, "admin@cartify.local"),
            new Claim("role", "Admin"),
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
