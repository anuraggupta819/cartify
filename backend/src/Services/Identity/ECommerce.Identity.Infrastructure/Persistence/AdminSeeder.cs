using ECommerce.Identity.Application.Abstractions;
using ECommerce.Identity.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Identity.Infrastructure.Persistence;

public static class AdminSeeder
{
    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        var configuration = services.GetRequiredService<IConfiguration>();
        var username = configuration["Admin:Username"];
        var password = configuration["Admin:Password"];

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var userRepository = services.GetRequiredService<IUserRepository>();
        var existing = await userRepository.GetByUsernameAsync(username, cancellationToken);
        if (existing is not null)
        {
            return;
        }

        var passwordHasher = services.GetRequiredService<IPasswordHasher>();
        var admin = User.CreateAdmin(username, configuration["Admin:Email"] ?? $"{username}@cartify.local", passwordHasher.Hash(password));

        await userRepository.AddAsync(admin, cancellationToken);

        var unitOfWork = services.GetRequiredService<IUnitOfWork>();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
