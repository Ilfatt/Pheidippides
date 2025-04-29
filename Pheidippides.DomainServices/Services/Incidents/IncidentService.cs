using Microsoft.EntityFrameworkCore;
using Pheidippides.Domain;
using Pheidippides.Domain.Exceptions;
using Pheidippides.DomainServices.Notifiers;
using Pheidippides.Infrastructure;

namespace Pheidippides.DomainServices.Services.Incidents;

public class IncidentService(AppDbContext appDbContext, IEnumerable<INotifier> notifiers)
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
                           .Include(x => x.AcknowledgedUsers)
                           .Include(x => x.Team)
                           .ThenInclude(x => x.Workers)
                           .Include(x => x.Team)
                           .ThenInclude(x => x.Lead)
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

    public async Task NotifyAboutIncidents(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var incidents = await appDbContext.Incidents
                .Include(x => x.AcknowledgedUsers)
                .Include(x => x.Team)
                .ThenInclude(x => x.Lead)
                .ThenInclude(x => x.Team)
                .OrderBy(x => x.Id)
                .Where(x => x.LastNotifiedMoment.AddSeconds(65) < DateTime.UtcNow && x.Level == 1
                            || x.LastNotifiedMoment == DateTimeOffset.MinValue && x.Level > 1)
                .Where(x =>
                    !x.AcknowledgedUsers.Select(user => user.Id).Contains(x.Team.DutyId!.Value)
                    || (!x.AcknowledgedUsers.Select(user => user.Id).Contains(x.Team.LeadId!.Value) && x.Team.LeadRotationRule == LeadRotationRule.LeadInRotation)
                    || !x.AdditionallyNeedAcknowledgedUsers.All(id => x.AcknowledgedUsers.Select(user => user.Id).Contains(id)))
                .Take(100)
                .ToListAsync(cancellationToken);

            if(incidents.Count == 0) return;

            foreach (var incident in incidents)
            {
                incident.LastNotifiedMoment = DateTimeOffset.UtcNow;
                
                var needAcknowledgedUsers = incident.AdditionallyNeedAcknowledgedUsers;
                needAcknowledgedUsers.Add(incident.Team.DutyId!.Value);

                if (incident.Team.LeadRotationRule == LeadRotationRule.LeadIsDuty)
                {
                    needAcknowledgedUsers.Add(incident.Team.LeadId!.Value);
                }
                
                needAcknowledgedUsers = needAcknowledgedUsers
                    .Except(incident.AcknowledgedUsers.Select(x => x.Id))
                    .ToList();

                var users = await appDbContext.Users.AsNoTracking()
                    .Where(x => needAcknowledgedUsers.Contains(x.Id))
                    .ToArrayAsync(cancellationToken);
                
                if (incident.Level == 1)
                {
                    await notifiers
                        .Single(x => x.NotifierType == NotifierType.YandexHomeStation)
                        .Notify(incident, users.Where(x => x is { YandexOAuthToken: not null, YandexScenarioName: not null }).ToArray(), cancellationToken);
                    
                    await notifiers
                        .Single(x => x.NotifierType == NotifierType.Phone)
                        .Notify(incident, users.Where(x => x is { YandexOAuthToken: null, YandexScenarioName: null }).ToArray(), cancellationToken);
                }
                else if (incident.Level == 2)
                {
                    await notifiers
                        .Single(x => x.NotifierType == NotifierType.Email)
                        .Notify(incident, users, cancellationToken);
                }
                else throw new NotImplementedException();
            }

            await appDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}