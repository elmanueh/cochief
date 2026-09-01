using Cochief.Domain.Enums;
using Cochief.Domain.Exceptions;
using Cochief.Domain.ValueObjects;
using System.Collections.ObjectModel;

namespace Cochief.Domain.Model;

public sealed class Clan
{
    private readonly List<Member> _members;

    public Guid Id { get; }
    public string Name { get; }
    public Tag Tag { get; }
    public ReadOnlyCollection<Member> Members { get; }

    private Clan(Guid id, string name, Tag tag)
    {
        Id = id;
        Name = name;
        Tag = tag;
        _members = [];
        Members = _members.AsReadOnly();
    }

    public static Clan Create(string name, string tag)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new InvalidClanException("Clan name cannot be empty.");
        Tag tagValue = Tag.Create(tag);

        return new Clan(Guid.NewGuid(), name.Trim(), tagValue);
    }

    public static Clan Restore(Guid id, string name, string tag, IEnumerable<Member>? members = null)
    {
        Clan clan = new Clan(id, name, Tag.Restore(tag));

        if (members is not null) clan._members.AddRange(members);

        return clan;
    }

    public void AddMember(Guid playerId)
    {
        if (playerId == Guid.Empty) throw new InvalidClanException("Player ID cannot be empty.");
        if (_members.Any(member => member.PlayerId == playerId)) throw new InvalidClanException("Player is already a member of the clan.");

        Member member = Member.Create(playerId, this.Id, MemberRole.Member);
        _members.Add(member);
    }

    public void RemoveMember(Guid playerId)
    {
        if (playerId == Guid.Empty) throw new InvalidClanException("Player ID cannot be empty.");

        Member? member = _members.FirstOrDefault(member => member.PlayerId == playerId);
        if (member == null) throw new InvalidClanException("Player is not a member of the clan.");

        _members.Remove(member);
    }
}
