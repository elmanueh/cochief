namespace Cochief.Domain.Events;

using Cochief.Domain.Shared;

public sealed class PlayerNameUpdatedEvent : DomainEvent
{
    public Guid PlayerId { get; }
    public string PreviousName { get; }
    public string Name { get; }

    public PlayerNameUpdatedEvent(Guid playerId, string previousName, string name)
    {
        PlayerId = playerId;
        PreviousName = previousName;
        Name = name;
    }
}
