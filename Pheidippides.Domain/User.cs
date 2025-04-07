namespace Pheidippides.Domain;

public class User
{
    public long Id { get; private init; }
    public string? Email { get; set; }
    public required string PhoneNumber { get; init; }
    public required string PasswordHash { get; set; }
    public required string FirstName { get; set; }
    public required string SecondName { get; set; }
    public required string Surname { get; set; }
    public required UserRole Role { get; set; }
    public string? YandexScenarioName { get; set; }
    public string? YandexOAuthToken { get; set; }
    public bool IsDuty { get; set; }
    public DateTimeOffset CreatedAt { get; private init; } = DateTimeOffset.UtcNow;
    public virtual Team? Team { get; init; }
    public virtual Team? LeadTeam { get; set; }
    public long TeamId { get; init; }
    public long LeadTeamId { get; init; }
}