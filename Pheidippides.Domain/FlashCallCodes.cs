namespace Pheidippides.Domain;

public class FlashCallCodes
{
    public long Id { get; init; }
    public required string PhoneNumber { get; init; }
    public required ushort Code { get; init; }
}