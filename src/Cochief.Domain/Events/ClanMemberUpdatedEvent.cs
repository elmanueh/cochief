namespace Cochief.Domain.Events;

using Cochief.Domain.Enums;
using Cochief.Domain.Shared;

public sealed class ClanMemberUpdatedEvent : DomainEvent
{
    public Guid ClanId { get; }
    public Guid PlayerId { get; }
    public MemberRole PreviousRole { get; }
    public MemberRole Role { get; }

    public ClanMemberUpdatedEvent(Guid clanId, Guid playerId, MemberRole previousRole, MemberRole role)
    {
        ClanId = clanId;
        PlayerId = playerId;
        PreviousRole = previousRole;
        Role = role;
    }
}
