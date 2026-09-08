namespace Cochief.Domain.Events;

using Cochief.Domain.Shared;

public sealed class UserPlayerUnlinkedEvent : DomainEvent
{
    public Guid UserId { get; }
    public Guid PlayerId { get; }

    public UserPlayerUnlinkedEvent(Guid userId, Guid playerId)
    {
        UserId = userId;
        PlayerId = playerId;
    }
}
