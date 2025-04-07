using Pheidippides.Domain;

namespace Pheidippides.DomainServices.Notifiers;

public interface INotifier
{
    Task Notify(Incident incident, User[] users, CancellationToken cancellationToken);
}