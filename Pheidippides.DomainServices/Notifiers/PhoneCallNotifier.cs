using Pheidippides.Domain;
using Pheidippides.DomainServices.Extensions;
using Pheidippides.ExternalServices;

namespace Pheidippides.DomainServices.Notifiers;

public class PhoneCallNotifier(ZvonokClient zvonokClient) : INotifier
{
    public NotifierType NotifierType => NotifierType.Phone;

    public async Task Notify(Incident incident, User[] users, CancellationToken cancellationToken)
    {
        await users.IndependentParallelForEachAsync(
            async (x, token) =>
            {
                await zvonokClient.AlertCall(x.PhoneNumber, "Алерт! " + incident.Title, token);
            },
            cancellationToken);
    }
}