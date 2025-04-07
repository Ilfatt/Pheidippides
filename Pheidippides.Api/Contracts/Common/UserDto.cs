namespace Pheidippides.Api.Contracts.Common;

public class UserDto
{
    public long Id { get; init; }
    public required string PhoneNumber { get; init; }
    public required string FirstName { get; init; }
    public required string SecondName { get; init; }
    public required string Surname { get; init; }
    public bool IsDuty { get; init; }
    public long TeamId { get; init; }
}