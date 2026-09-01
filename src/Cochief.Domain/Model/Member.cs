using Cochief.Domain.Enums;
using Cochief.Domain.Exceptions;

namespace Cochief.Domain.Model;

public sealed class Member
{
    public Guid Id { get; }
    public Guid PlayerId { get; }
    public Guid ClanId { get; }
    public MemberRole Role { get; private set; }

    private Member(Guid id, Guid playerId, Guid clanId, MemberRole role)
    {
        Id = id;
        PlayerId = playerId;
        ClanId = clanId;
        Role = role;
    }

    public static Member Create(Guid playerId, Guid clanId, MemberRole role)
    {
        if (playerId == Guid.Empty) throw new InvalidMemberException("Member player cannot be empty.");
        if (clanId == Guid.Empty) throw new InvalidMemberException("Member clan cannot be empty.");

        return new Member(Guid.NewGuid(), playerId, clanId, role);
    }

    public static Member Restore(Guid id, Guid playerId, Guid clanId, MemberRole role)
    {
        return new Member(id, playerId, clanId, role);
    }

    public void ChangeRole(MemberRole newRole)
    {
        Role = newRole;
    }
}
