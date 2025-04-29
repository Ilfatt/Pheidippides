namespace Pheidippides.Api.Contracts.User;

public class UpdateProfileInfoDto
{
    public string Email { get; init; } = null!;
    public string YandexScenarioId { get; init; } = null!;
    public string YandexOAuthToken { get; init; } = null!;
}