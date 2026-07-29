namespace ECommerce.Identity.Domain.Entities;

public enum UserRole
{
    Admin,
    Customer
}

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string? GoogleSub { get; private set; }
    public string? Username { get; private set; }
    public string? PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private User() { }

    public static User CreateGoogleCustomer(string email, string googleSub, string name)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(googleSub))
        {
            throw new ArgumentException("Google subject id is required.", nameof(googleSub));
        }

        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            GoogleSub = googleSub,
            Username = name,
            Role = UserRole.Customer,
            CreatedAtUtc = DateTime.UtcNow,
        };
    }

    public static User CreateAdmin(string username, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required.", nameof(username));
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));
        }

        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Username = username,
            PasswordHash = passwordHash,
            Role = UserRole.Admin,
            CreatedAtUtc = DateTime.UtcNow,
        };
    }
}
