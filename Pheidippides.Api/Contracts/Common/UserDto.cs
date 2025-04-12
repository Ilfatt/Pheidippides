using Pheidippides.Domain;

namespace Pheidippides.Api.Contracts.Common;

public class UserDto
{
    public long Id { get; init; }
    public required string PhoneNumber { get; init; }
    public required string FirstName { get; init; }
    public required string SecondName { get; init; }
    public required UserRole Role { get; init; }
    public required string Surname { get; init; }
    
    public required string? YandexScenarioName { get; init; }
    public required string? YandexOAuthToken { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required string? Email { get; init; }

    public bool IsDuty { get; init; }
    public long TeamId { get; init; }
}