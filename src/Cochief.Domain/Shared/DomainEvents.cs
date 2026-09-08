namespace Cochief.Domain.Shared;

public sealed class DomainEvents
{
    private readonly Dictionary<Type, List<Func<IDomainEvent, CancellationToken, Task>>> _handlers = [];
    private readonly List<Func<IDomainEvent, CancellationToken, Task>> _globalHandlers = [];

    public void Register<TEvent>(Func<TEvent, CancellationToken, Task> handler)
        where TEvent : IDomainEvent
    {
        Type eventType = typeof(TEvent);
        if (!_handlers.TryGetValue(eventType, out List<Func<IDomainEvent, CancellationToken, Task>>? handlers))
        {
            handlers = [];
            _handlers.Add(eventType, handlers);
        }

        handlers.Add((domainEvent, ct) => handler((TEvent)domainEvent, ct));
    }

    public void RegisterGlobal(Func<IDomainEvent, CancellationToken, Task> handler)
    {
        _globalHandlers.Add(handler);
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken ct)
    {
        foreach (IDomainEvent domainEvent in domainEvents)
        {
            foreach (Func<IDomainEvent, CancellationToken, Task> handler in _globalHandlers)
            {
                await handler(domainEvent, ct);
            }

            if (!_handlers.TryGetValue(domainEvent.GetType(), out List<Func<IDomainEvent, CancellationToken, Task>>? handlers))
            {
                continue;
            }

            foreach (Func<IDomainEvent, CancellationToken, Task> handler in handlers)
            {
                await handler(domainEvent, ct);
            }
        }
    }
}
