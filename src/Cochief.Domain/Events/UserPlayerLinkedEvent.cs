namespace Cochief.Domain.Events;

using Cochief.Domain.Shared;

public sealed class UserPlayerLinkedEvent : DomainEvent
{
    public Guid UserId { get; }
    public Guid PlayerId { get; }
    public string? ClanTag { get; }

    public UserPlayerLinkedEvent(Guid userId, Guid playerId, string? clanTag)
    {
        UserId = userId;
        PlayerId = playerId;
        ClanTag = clanTag;
    }
}
