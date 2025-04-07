namespace Pheidippides.Domain;

public class Team
{
    public long Id { get; private init; }
    public required string Name { get; set; }
    public required string InviteToken { get; set; }
    public virtual required User Lead { get; init; }
    public long LeadId { get; private init; }
    public virtual List<User> Workers { get; init; } = [];
    public virtual List<Incident> Incidents { get; init; } = [];
}