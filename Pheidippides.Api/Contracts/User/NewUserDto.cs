namespace Pheidippides.Api.Contracts.User;

public class NewUserDto
{
    public string FirstName { get; init; } = null!;
    public string SecondName { get; init; } = null!;
    public string Surname { get; init; } = null!;
    public string PhoneNumber { get; init; } = null!;
    public ushort PhoneActivationCode { get; init; }
    public string Password { get; init; } = null!;
    public string? TeamName { get; init; }
    public string? TeamInviteCode { get; init; }
}