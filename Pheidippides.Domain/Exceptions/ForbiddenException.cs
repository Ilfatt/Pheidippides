namespace Pheidippides.Domain.Exceptions;

public class ForbiddenException(string message, Exception? exception = null) : DomainException(message, 403, exception);