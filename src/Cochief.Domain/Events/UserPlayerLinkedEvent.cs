namespace Cochief.Domain.Events;

using Cochief.Domain.Shared;

public sealed class UserPlayerLinkedEvent : DomainEvent
{
    public Guid UserId { get; }
    public Guid PlayerId { get; }

    public UserPlayerLinkedEvent(Guid userId, Guid playerId)
    {
        UserId = userId;
        PlayerId = playerId;
    }
}
