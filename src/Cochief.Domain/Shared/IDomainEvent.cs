namespace Cochief.Domain.Shared;

using MediatR;

public interface IDomainEvent : INotification
{
    Guid Id { get; }
    DateTimeOffset OccurredOn { get; }
    string Type { get; }
}
