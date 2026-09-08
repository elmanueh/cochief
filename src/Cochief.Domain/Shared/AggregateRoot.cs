namespace Cochief.Domain.Shared;

public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot(Guid? id) : base(id)
    {
    }

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public IReadOnlyCollection<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents.AsReadOnly();
    }

    public IReadOnlyCollection<IDomainEvent> PullDomainEvents()
    {
        IDomainEvent[] domainEvents = _domainEvents.ToArray();
        _domainEvents.Clear();

        return domainEvents;
    }
}
