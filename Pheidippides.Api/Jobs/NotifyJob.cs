using Pheidippides.DomainServices.Services.Incidents;

namespace Pheidippides.Api.Jobs;

public class NotifyJob(IServiceProvider serviceProvider, ILogger<NotifyJob> logger) : BackgroundService
{
    protected override  async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
     
        await using var scope = serviceProvider.CreateAsyncScope();
        var incidentService = scope.ServiceProvider.GetRequiredService<IncidentService>();
        
        while (stoppingToken.IsCancellationRequested == false)
        {
            await Task.Delay(2 * 1000, stoppingToken);

            try
            {
                await incidentService.NotifyAboutIncidents(stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed notify about incidents");
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }
}