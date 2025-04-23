namespace Pheidippides.Api.Contracts.Incidents;

public class CreateNewIncidentDto
{
    public required string Title { get; init; } = null!;
    public required string Description { get; init; } = null!;
    public required ushort Level { get; init; }
    public required string IncidentCreateToken { get; init; }
}