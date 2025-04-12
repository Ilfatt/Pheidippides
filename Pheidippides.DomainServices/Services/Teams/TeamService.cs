using Microsoft.EntityFrameworkCore;
using Pheidippides.Domain;
using Pheidippides.DomainServices.Services.User;
using Pheidippides.Infrastructure;

namespace Pheidippides.DomainServices.Services.Teams;

public class TeamService(AppDbContext appDbContext, UserService userService)
{
    public async Task<Team> GetUserTeamWithMember(long userId, CancellationToken cancellationToken)
    {
        var team = await appDbContext.Teams.AsNoTracking()
            .Include(x => x.Lead)
            .Include(x => x.Workers)
            .FirstOrDefaultAsync(
                x => x.LeadId == userId || x.Workers.Any(worker => worker.Id == userId),
                cancellationToken);

        if (team == null)
        {
            await userService.ThrowIfUserNotExist(userId, cancellationToken: cancellationToken);
        }

        return team ?? throw new InvalidDataException("User team not found"); 
    }
}