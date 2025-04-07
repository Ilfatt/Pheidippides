namespace Pheidippides.Domain.Exceptions;

public abstract class DomainException(
    string message,
    ushort statusCode,
    Exception? exception = null) : Exception(message, exception)
{
    public ushort StatusCode { get; } = statusCode;
    public Dictionary<string, string> PlaceholderData { get; } = new();
}