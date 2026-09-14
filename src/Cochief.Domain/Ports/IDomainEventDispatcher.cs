namespace Cochief.Domain.Ports;

using Cochief.Domain.Shared;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken ct);
}
