namespace Pheidippides.DomainServices.Services.User;

public class RegisterCommand
{
    public required string FirstName { get; init; } 
    public required string SecondName { get; init; } 
    public required string Surname { get; init; } 
    public required string PhoneNumber { get; init; }
    public required ushort PhoneActivationCode { get; init; }
    public required string Password { get; init; }
    public required string? TeamName { get; init; }
    public required string? TeamInviteCode { get; init; }
}