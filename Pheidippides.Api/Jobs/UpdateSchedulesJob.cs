using Pheidippides.DomainServices.Services.Schedules;

namespace Pheidippides.Api.Jobs;

public class UpdateSchedulesJob(IServiceProvider serviceProvider, ILogger<UpdateSchedulesJob> logger) : BackgroundService 
{
    protected override  async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
     
        await using var scope = serviceProvider.CreateAsyncScope();
        var scheduleService = scope.ServiceProvider.GetRequiredService<ScheduleService>();
        
        while (stoppingToken.IsCancellationRequested == false)
        {
            await Task.Delay(10 * 1000, stoppingToken);

            try
            {
                await scheduleService.UpdateSchedules(stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to update schedules");
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }
}