using Microsoft.EntityFrameworkCore;
using Pheidippides.Domain;
using Pheidippides.Infrastructure;

namespace Pheidippides.DomainServices.Services.Schedules;

public class ScheduleService(AppDbContext appDbContext)
{
    public async Task UpdateSchedules(CancellationToken cancellationToken)
    {
        while (true)
        {
            var teams = await appDbContext.Teams
                .Include(x => x.Lead)
                .Include(x => x.Workers)
                .Where(x => x.LeadRotationRule != LeadRotationRule.LeadIsNotDuty || x.Workers.Any())
                .Where(x =>
                    x.LastRotationChange == null
                    || (x.LastRotationChange.Value.DayOfYear + x.RotationPeriodInDays <= DateTimeOffset.UtcNow.DayOfYear
                        && DateTimeOffset.UtcNow.TimeOfDay > x.RotationStartTime))
                .OrderBy(x => 1)
                .Take(1000)
                .ToListAsync(cancellationToken);
            
            teams.AddRange(await appDbContext.Teams
                .Include(x => x.Lead)
                .Include(x => x.Workers)
                .Where(x => x.LeadId == x.DutyId && x.LeadRotationRule != LeadRotationRule.LeadInRotation)
                .Where(x => x.Workers.Any())
                .ToListAsync(cancellationToken));

            if (teams.Count == 0)
                return;

            foreach (var team in teams)
            {
                team.DutyId = GetDutyOrder(team, cancellationToken).First(x => x != team.DutyId);
                team.LastRotationChange = DateTimeOffset.UtcNow;
            }

            await appDbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<Schedule> GetSchedule(long userId, CancellationToken cancellationToken)
    {
        var team = await appDbContext.Teams.AsNoTracking()
                       .Include(x => x.Lead)
                       .Include(x => x.Workers)
                       .FirstOrDefaultAsync(
                           x => x.LeadId == userId || x.Workers.Any(user => user.Id == userId),
                           cancellationToken)
                   ?? throw new ArgumentException(null, nameof(userId));

        var point = DateTimeOffset.UtcNow.Date
            .AddDays(-team.RotationPeriodInDays)
            .Add(team.RotationStartTime);

        using var order = GetDutyOrder(team, cancellationToken).GetEnumerator();

        var items = Enumerable.Range(0, 31 / team.RotationPeriodInDays)
            .Select(_ =>
            {
                point = point.AddDays(team.RotationPeriodInDays);
                order.MoveNext();

                return new ScheduleItem
                {
                    From = point.AddDays(-1),
                    To = point.AddDays(team.RotationPeriodInDays),
                    UserId = order.Current,
                };
            })
            .ToList();

        if (items.DistinctBy(x => x.UserId).Count() == 1)
        {
            items =
            [
                new ScheduleItem
                {
                    From = items.Min(x => x.From),
                    To = items.Max(x => x.From),
                    UserId = items.First().UserId
                }
            ];
        }

        return new Schedule { ScheduleItems = items };
    }

    public static IEnumerable<long> GetDutyOrder(Team team, CancellationToken cancellationToken)
    {
        if (team.DutyId.HasValue)
            yield return team.DutyId!.Value;

        var order = team.Workers.Select(x => x).ToList();

        if (team.LeadRotationRule == LeadRotationRule.LeadInRotation || team.Workers.Count == 0)
        {
            order = order.Append(team.Lead).ToList();
        }

        order = order.OrderBy(x => x.Id).ToList();

        foreach (var worker in order)
        {
            if (!team.DutyId.HasValue || worker.Id <= team.DutyId!.Value)
                continue;

            cancellationToken.ThrowIfCancellationRequested();
            yield return worker.Id;
        }

        for (var i = 0; i < 20; i++)
        {
            foreach (var worker in order)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return worker.Id;
            }
        }
    }
}