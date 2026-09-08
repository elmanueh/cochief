using Cochief.Domain.Enums;
using Cochief.Domain.Events;
using Cochief.Domain.Exceptions;
using Cochief.Domain.Shared;
using Cochief.Domain.ValueObjects;
using System.Collections.ObjectModel;

namespace Cochief.Domain.Model;

public sealed class Clan : AggregateRoot
{
    private readonly List<Member> _members;

    public string Name { get; }
    public Tag Tag { get; }
    public ReadOnlyCollection<Member> Members { get; }

    private Clan(string name, Tag tag, Guid? id = null) : base(id)
    {
        Name = name;
        Tag = tag;
        _members = [];
        Members = _members.AsReadOnly();
    }

    public static Clan Create(string name, string tag)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new InvalidClanException("Clan name cannot be empty.");
        Tag tagValue = Tag.Create(tag);

        Clan clan = new Clan(name.Trim(), tagValue);
        clan.AddDomainEvent(new ClanCreatedEvent(clan.Id, clan.Name, clan.Tag.Value));

        return clan;
    }

    public static Clan Restore(Guid id, string name, string tag)
    {
        return new Clan(name, Tag.Restore(tag), id);
    }

    public void AddMember(Guid playerId, MemberRole role)
    {
        if (_members.Any(member => member.PlayerId == playerId)) throw new InvalidClanException("Player is already a member of the clan.");

        Member member = Member.Create(playerId, Id, role);
        _members.Add(member);
        this.AddDomainEvent(new ClanMemberAddedEvent(Id, playerId, role));
    }

    public void UpdateMember(Guid playerId, MemberRole role)
    {
        Member member = _members.FirstOrDefault(member => member.PlayerId == playerId)
            ?? throw new InvalidClanException("Player is not a member of the clan.");

        if (member.Role == role) return;

        MemberRole previousRole = member.Role;
        member.ChangeRole(role);
        this.AddDomainEvent(new ClanMemberUpdatedEvent(Id, playerId, previousRole, role));
    }

    public void DeleteMember(Guid playerId)
    {
        Member member = _members.FirstOrDefault(member => member.PlayerId == playerId)
            ?? throw new InvalidClanException("Player is not a member of the clan.");

        _members.Remove(member);
        this.AddDomainEvent(new ClanMemberDeletedEvent(Id, playerId, member.Role));
    }
}
