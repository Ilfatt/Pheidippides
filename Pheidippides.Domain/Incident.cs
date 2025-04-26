namespace Pheidippides.Domain;

public class Incident
{
    public long Id { get; private init; }
    public required string Title { get; init; } = null!;
    public required string Description { get; init; } = null!;
    public required ushort Level { get; init; }
    public required long TeamId { get; init; }
    public DateTimeOffset LastNotifiedMoment { get; set; } = DateTimeOffset.MinValue;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public List<long> AdditionallyNeedAcknowledgedUsers { get; init; } = [];
    public virtual Team Team { get; init; } = null!;
    public virtual List<User> AcknowledgedUsers { get; init; } = null!;
}