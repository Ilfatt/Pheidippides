namespace Pheidippides.Domain.Exceptions;

public class ConflictException(string message, Exception? exception = null) : DomainException(message, 409, exception);