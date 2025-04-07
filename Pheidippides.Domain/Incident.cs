namespace Pheidippides.Domain;

public class Incident
{
    public long Id { get; private init; }
    public required string Title { get; init; } = null!;
    public required string Description { get; init; } = null!;
    public bool IsClosed { get; set; } = false;
    public required bool IsCritical { get; init; }
    public required long TeamId { get; init; }
    public DateTimeOffset LastNotifiedMoment { get; init; } = DateTimeOffset.MinValue;
    public virtual Team Team { get; init; } = null!;
    public virtual List<User> AcknowledgedUsers { get; init; } = null!;
}