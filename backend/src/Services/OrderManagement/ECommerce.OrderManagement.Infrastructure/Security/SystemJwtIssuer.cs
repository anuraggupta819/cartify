using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ECommerce.OrderManagement.Infrastructure.Security;

// Mints a short-lived token for outbound calls that happen outside an HTTP request
// (the abandoned-order background sweep), using the same shared signing key every
// service already trusts — no separate service-account secret needed.
public class SystemJwtIssuer(IConfiguration configuration)
{
    public string IssueToken()
    {
        var jwtKey = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Configuration 'Jwt:Key' is not set.");
        var jwtIssuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("Configuration 'Jwt:Issuer' is not set.");
        var jwtAudience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("Configuration 'Jwt:Audience' is not set.");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "system-ordermanagement"),
            new Claim("role", "System"),
        };

        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(jwtIssuer, jwtAudience, claims, expires: DateTime.UtcNow.AddMinutes(5), signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
