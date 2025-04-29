using Pheidippides.Domain;

namespace Pheidippides.DomainServices.Notifiers;

public interface INotifier
{
    public  NotifierType NotifierType { get; }
    Task Notify(Incident incident, User[] users, CancellationToken cancellationToken);
}