using ECommerce.Identity.Application.Abstractions;
using ECommerce.Identity.Application.Dtos;
using ECommerce.Identity.Application.Exceptions;
using ECommerce.Identity.Domain.Entities;

namespace ECommerce.Identity.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IGoogleTokenValidator googleTokenValidator,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator)
{
    public async Task<AuthResponse> GoogleLoginAsync(GoogleLoginRequest request, CancellationToken cancellationToken = default)
    {
        var googleUser = await googleTokenValidator.ValidateAsync(request.IdToken, cancellationToken);

        var user = await userRepository.GetByGoogleSubAsync(googleUser.Sub, cancellationToken);
        if (user is null)
        {
            user = User.CreateGoogleCustomer(googleUser.Email, googleUser.Sub, googleUser.Name ?? googleUser.Email);
            await userRepository.AddAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var token = jwtTokenGenerator.GenerateToken(user);
        return new AuthResponse(token, user.Email, user.Username, user.Role.ToString());
    }

    public async Task<AuthResponse> AdminLoginAsync(AdminLoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByUsernameAsync(request.Username, cancellationToken)
            ?? throw new AuthenticationFailedException("Invalid username or password.");

        if (user.PasswordHash is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new AuthenticationFailedException("Invalid username or password.");
        }

        var token = jwtTokenGenerator.GenerateToken(user);
        return new AuthResponse(token, user.Email, user.Username, user.Role.ToString());
    }
}
