using Pheidippides.Api.Contracts.Common;
using Pheidippides.Domain;

namespace Pheidippides.Api.Contracts.Team;

public class TeamDto
{
    public long Id { get; init; }
    public required string Name { get; init; }
    public required string InviteToken { get; init; }
    public required LeadRotationRule LeadRotationRule { get; init; }
    public required UserDto Lead { get; init; }
    public required List<UserDto> Workers { get; init; }
}