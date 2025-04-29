using Microsoft.Extensions.Logging;
using Pheidippides.Domain;
using Pheidippides.DomainServices.Extensions;
using Pheidippides.ExternalServices;

namespace Pheidippides.DomainServices.Notifiers;

public class YandexHomeStationNotifier(YandexApiClient yandexApi, ILogger<YandexHomeStationNotifier> logger) : INotifier
{
    public NotifierType NotifierType => NotifierType.YandexHomeStation;

    public async Task Notify(Incident incident, User[] users, CancellationToken cancellationToken)
    {
        await users.IndependentParallelForEachAsync(
            async (x, token) =>
            {
                var scenarioId = x.YandexScenarioName!.Split(":")[1];
                
                await yandexApi.ExecuteScenario(scenarioId, x.YandexOAuthToken!, token);
            },
            logger,
            cancellationToken);
    }
}