using Cochief.Domain.Enums;
using Cochief.Domain.Exceptions;
using Cochief.Domain.ValueObjects;

namespace Cochief.Domain.Model;

public sealed class Clan
{
    private Guid Id { get; }
    private string Name { get; }
    private Tag Tag { get; }
    private IEnumerable<Member> Members { get; }

    private Clan(Guid id, string name, Tag tag)
    {
        Id = id;
        Name = name;
        Tag = tag;
        Members = new List<Member>();
    }

    public static Clan Create(string name, string tag)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new InvalidClanException("Clan name cannot be empty.");
        Tag tagValue = Tag.Create(tag);

        return new Clan(Guid.NewGuid(), name.Trim(), tagValue);
    }

    public void AddMember(Guid playerId)
    {
        if (playerId == Guid.Empty) throw new InvalidClanException("Player ID cannot be empty.");
        if (Members.Any(m => m.GetPlayerId() == playerId)) throw new InvalidClanException("Player is already a member of the clan.");

        Member member = Member.Create(playerId, this.Id, MemberRole.Member);
        ((List<Member>)Members).Add(member);
    }

    public void RemoveMember(Guid playerId)
    {
        if (playerId == Guid.Empty) throw new InvalidClanException("Player ID cannot be empty.");

        Member? member = Members.FirstOrDefault(m => m.GetPlayerId() == playerId);
        if (member == null) throw new InvalidClanException("Player is not a member of the clan.");

        ((List<Member>)Members).Remove(member);
    }
}
