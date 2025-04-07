namespace Pheidippides.Domain.Exceptions;

public class UnauthorizedException(
    string message,
    Exception? exception = null) : DomainException(message, 401, exception);