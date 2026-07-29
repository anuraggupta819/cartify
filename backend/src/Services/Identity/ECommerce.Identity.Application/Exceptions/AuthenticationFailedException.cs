namespace ECommerce.Identity.Application.Exceptions;

public class AuthenticationFailedException(string message, Exception? innerException = null)
    : Exception(message, innerException);
