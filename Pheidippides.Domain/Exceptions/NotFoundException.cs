namespace Pheidippides.Domain.Exceptions;

public class NotFoundException(string message, Exception? exception = null) : DomainException(message, 404, exception);