using Pheidippides.Domain;

namespace Pheidippides.DomainServices.Notifiers;

public class EmailNotifier : INotifier
{
    public NotifierType NotifierType => NotifierType.Email;
    
    public Task Notify(Incident incident, User[] users, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}