using ECommerce.Identity.Domain.Entities;

namespace ECommerce.Identity.Application.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByGoogleSubAsync(string googleSub, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
}
