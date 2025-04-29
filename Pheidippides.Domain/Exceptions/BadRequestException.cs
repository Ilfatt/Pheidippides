namespace Pheidippides.Domain.Exceptions;

public class BadRequestException(string message, Exception? exception = null) : DomainException(message, 400, exception)
{
    
}