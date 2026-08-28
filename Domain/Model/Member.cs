using Cochief.Domain.Enums;
using Cochief.Domain.Exceptions;

namespace Cochief.Domain.Model;

public sealed class Member
{
    private Guid Id { get; }
    private Guid PlayerId { get; }
    private Guid ClanId { get; }
    private MemberRole Role { get; set; }

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

    public Guid GetPlayerId() => PlayerId;

    public void ChangeRole(MemberRole newRole)
    {
        Role = newRole;
    }
}
