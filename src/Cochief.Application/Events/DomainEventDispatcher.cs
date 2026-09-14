namespace Cochief.Application.Events;

using Cochief.Domain.Ports;
using Cochief.Domain.Shared;
using MediatR;

public sealed class DomainEventDispatcher(IPublisher publisher) : IDomainEventDispatcher
{
    private readonly IPublisher _publisher = publisher;

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken ct)
    {
        foreach (IDomainEvent domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, ct);
        }
    }
}
