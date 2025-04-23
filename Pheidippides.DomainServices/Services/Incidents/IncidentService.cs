using Microsoft.EntityFrameworkCore;
using Pheidippides.Domain;
using Pheidippides.Domain.Exceptions;
using Pheidippides.Infrastructure;

namespace Pheidippides.DomainServices.Services.Incidents;

public class IncidentService(AppDbContext appDbContext)
{
    public async Task CreateIncident(CreateNewIncidentCommand command, CancellationToken cancellationToken)
    {
        var teamId = await appDbContext.Teams
                         .Where(x => x.IncidentCreateToken == command.IncidentCreateToken)
                         .Select(x => (long?)x.Id)
                         .FirstOrDefaultAsync(cancellationToken)
                     ?? throw new NotFoundException(
                         $"Team with IncidentCreateToken = {command.IncidentCreateToken} not found");

        var incident = new Incident
        {
            Title = command.Title,
            Description = command.Description,
            Level = command.Level,
            TeamId = teamId
        };

        appDbContext.Incidents.Add(incident);
        await appDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Acknowledge(long userId, long incidentId, CancellationToken cancellationToken)
    {
        var incident = await appDbContext.Incidents
                           .Include(x => x.Team)
                           .ThenInclude(x => x.Workers)
                           .Include(x => x.Team)
                           .ThenInclude(x => x.LeadId)
                           .FirstOrDefaultAsync(x => x.Id == incidentId, cancellationToken)
                       ?? throw new NotFoundException($"Incident with Id = {incidentId} not found");

        var user = await appDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
                   ?? throw new ArgumentException($"User with Id = {userId} not found");

        if ((incident.Team.LeadRotationRule != LeadRotationRule.LeadIsDuty || incident.Team.LeadId != user.Id)
            && incident.Team.DutyId != userId
            && !incident.AdditionallyNeedAcknowledgedUsers.Contains(user.Id))
            throw new BadRequestException("The user was not called");

        incident.AcknowledgedUsers.Add(user);

        await appDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Incident[]> GetHistory(long userId, CancellationToken cancellationToken)
    {
        var incidents = await appDbContext.Incidents.AsNoTracking()
            .Include(x => x.AcknowledgedUsers)
            .Include(x => x.Team)
            .ThenInclude(x => x.Workers)
            .Include(x => x.Team)
            .ThenInclude(x => x.Lead)
            .Where(x => x.Team.LeadId == userId || x.Team.Workers.Any(user => user.Id == userId))
            .OrderByDescending(x => x.CreatedAt)
            .Take(30)
            .ToArrayAsync(cancellationToken);

        return incidents;
    }
}