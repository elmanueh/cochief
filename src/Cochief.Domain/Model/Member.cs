using Cochief.Domain.Enums;
using Cochief.Domain.Exceptions;
using Cochief.Domain.Shared;

namespace Cochief.Domain.Model;

public sealed class Member : Entity
{
    public Guid PlayerId { get; }
    public Guid ClanId { get; }
    public MemberRole Role { get; private set; }

    private Member(Guid playerId, Guid clanId, MemberRole role, Guid? id = null) : base(id)
    {
        PlayerId = playerId;
        ClanId = clanId;
        Role = role;
    }

    internal static Member Create(Guid playerId, Guid clanId, MemberRole role)
    {
        if (playerId == Guid.Empty) throw new InvalidMemberException("Member player cannot be empty.");
        if (clanId == Guid.Empty) throw new InvalidMemberException("Member clan cannot be empty.");

        return new Member(playerId, clanId, role);
    }

    internal void ChangeRole(MemberRole newRole)
    {
        Role = newRole;
    }
}
