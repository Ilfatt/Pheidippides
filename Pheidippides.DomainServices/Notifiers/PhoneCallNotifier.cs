using Pheidippides.Domain;

namespace Pheidippides.DomainServices.Notifiers;

public class PhoneCallNotifier : INotifier
{
    public Task Notify(Incident incident, User[] users, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}