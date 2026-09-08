namespace Cochief.Domain.Events;

using Cochief.Domain.Enums;
using Cochief.Domain.Shared;

public sealed class ClanMemberAddedEvent : DomainEvent
{
    public Guid ClanId { get; }
    public Guid PlayerId { get; }
    public MemberRole Role { get; }

    public ClanMemberAddedEvent(Guid clanId, Guid playerId, MemberRole role)
    {
        ClanId = clanId;
        PlayerId = playerId;
        Role = role;
    }
}
