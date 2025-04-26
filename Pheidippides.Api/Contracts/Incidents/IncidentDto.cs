namespace Pheidippides.Api.Contracts.Incidents;

public class IncidentDto
{
    public required long Id { get; init; }
    public required string Title { get; init; } = null!;
    public required string Description { get; init; } = null!;
    public required ushort Level { get; init; }
    public required DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public required bool NeedAcknowledgeCurrentUser { get; init; }
}