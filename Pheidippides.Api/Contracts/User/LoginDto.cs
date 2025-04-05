namespace Pheidippides.Api.Contracts.User;

public class LoginDto
{
    public string PhoneNumber { get; init; } = null!;
    public string Password { get; init; } = null!;
}