namespace Cochief.Domain.Shared;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTimeOffset OccurredOn { get; }
    string Type { get; }
}
