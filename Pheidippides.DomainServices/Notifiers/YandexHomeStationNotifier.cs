using Pheidippides.Domain;
using Pheidippides.DomainServices.Extensions;
using Pheidippides.ExternalServices;

namespace Pheidippides.DomainServices.Notifiers;

public class YandexHomeStationNotifier(YandexApiClient yandexApi) : INotifier
{
    public NotifierType NotifierType => NotifierType.YandexHomeStation;

    public async Task Notify(Incident incident, User[] users, CancellationToken cancellationToken)
    {
        await users.IndependentParallelForEachAsync(
            async (x, token) =>
            {
                await yandexApi.ExecuteScenario(x.YandexScenarioName!, x.YandexOAuthToken!, token);
            },
            cancellationToken);
    }
}