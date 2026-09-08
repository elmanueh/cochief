namespace Cochief.Domain.Events;

using Cochief.Domain.Shared;

public sealed class PlayerClanUpdatedEvent : DomainEvent
{
    public Guid PlayerId { get; }
    public Guid? PreviousClanId { get; }
    public Guid? ClanId { get; }

    public PlayerClanUpdatedEvent(Guid playerId, Guid? previousClanId, Guid? clanId)
    {
        PlayerId = playerId;
        PreviousClanId = previousClanId;
        ClanId = clanId;
    }
}
