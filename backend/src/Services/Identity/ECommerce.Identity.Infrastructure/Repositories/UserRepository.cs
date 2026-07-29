using ECommerce.Identity.Application.Abstractions;
using ECommerce.Identity.Domain.Entities;
using ECommerce.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Identity.Infrastructure.Repositories;

public class UserRepository(IdentityDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByGoogleSubAsync(string googleSub, CancellationToken cancellationToken = default) =>
        dbContext.Users.FirstOrDefaultAsync(u => u.GoogleSub == googleSub, cancellationToken);

    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        dbContext.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default) =>
        await dbContext.Users.AddAsync(user, cancellationToken);
}
