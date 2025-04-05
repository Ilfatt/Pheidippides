namespace Pheidippides.Domain;

public class Team
{
    public long Id { get; private init; }
    public required string Name { get; set; }
    public required string InviteToken { get; set; }
    public virtual required User Lead { get; set; }
    public required long LeadId { get; set; }
    public virtual required List<User> Workers { get; init; }
}