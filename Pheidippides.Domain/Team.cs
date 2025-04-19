namespace Pheidippides.Domain;

public class Team
{
    public long Id { get; private init; }
    public required string Name { get; set; }
    public required string InviteToken { get; set; }
    public LeadRotationRule LeadRotationRule { get; set; }
    public virtual required User Lead { get; init; }
    public ushort RotationPeriodInDays { get; set; } = 3;
    public DateTimeOffset? LastRotationChange { get; set; }
    public TimeSpan RotationStartTime { get; set; } = new(10, 0, 0);
    public long? DutyId { get; set; }
    public long? LeadId { get; private init; }
    public virtual List<User> Workers { get; init; } = [];
    public virtual List<Incident> Incidents { get; init; } = [];
}