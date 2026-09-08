namespace Cochief.Domain.Shared;

public abstract class DomainEvent : IDomainEvent
{
    public Guid Id { get; }
    public DateTimeOffset OccurredOn { get; }
    public string Type => GetType().Name;

    protected DomainEvent()
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTimeOffset.UtcNow;
    }
}
