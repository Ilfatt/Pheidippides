using Microsoft.EntityFrameworkCore;
using Pheidippides.Domain;
using Pheidippides.DomainServices.Services.Schedules;
using Pheidippides.DomainServices.Services.User;
using Pheidippides.Infrastructure;

namespace Pheidippides.DomainServices.Services.Teams;

public class TeamService(AppDbContext appDbContext, UserService userService, ScheduleService scheduleService)
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

    public async Task SetLeadRotationRule(
        long leadId, 
        LeadRotationRule leadRotationRule,
        CancellationToken cancellationToken)
    {
        var team = await appDbContext.Teams.FirstOrDefaultAsync(x => x.LeadId == leadId, cancellationToken);
        
        ArgumentNullException.ThrowIfNull(team);
        
        team.LeadRotationRule = leadRotationRule;
        
        await appDbContext.SaveChangesAsync(cancellationToken);
        await scheduleService.UpdateSchedules(cancellationToken);
    }
}