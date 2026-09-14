namespace Cochief.Domain.Events;

using Cochief.Domain.Shared;

public sealed class PlayerClanUpdatedEvent : DomainEvent
{
    public Guid PlayerId { get; }
    public string? PreviousClanTag { get; }
    public string? ClanTag { get; }

    public PlayerClanUpdatedEvent(Guid playerId, string? previousClanTag, string? clanTag)
    {
        PlayerId = playerId;
        PreviousClanTag = previousClanTag;
        ClanTag = clanTag;
    }
}
