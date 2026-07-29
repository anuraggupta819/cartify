using ECommerce.Identity.Domain.Entities;

namespace ECommerce.Identity.Application.Abstractions;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
