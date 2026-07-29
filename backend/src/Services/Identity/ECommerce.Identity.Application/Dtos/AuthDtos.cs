namespace ECommerce.Identity.Application.Dtos;

public record GoogleLoginRequest(string IdToken);

public record AdminLoginRequest(string Username, string Password);

public record AuthResponse(string Token, string Email, string? Name, string Role);
