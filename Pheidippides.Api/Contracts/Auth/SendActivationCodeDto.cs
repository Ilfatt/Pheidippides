namespace Pheidippides.Api.Contracts.Auth;

public class SendActivationCodeDto
{
    public string PhoneNumber { get; init; } = null!;
}