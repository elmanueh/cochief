using Cochief.Domain.Events;
using Cochief.Domain.Exceptions;
using Cochief.Domain.Shared;
using Cochief.Domain.ValueObjects;

namespace Cochief.Domain.Model;

public sealed class Player : AggregateRoot
{
    public Tag Tag { get; }
    public string Name { get; private set; }
    public int TownHallLevel { get; private set; }
    public Tag? ClanTag { get; private set; }

    private Player(string name, Tag tag, int townHallLevel, Tag? clanTag = null, Guid? id = null) : base(id)
    {
        Name = name;
        Tag = tag;
        TownHallLevel = townHallLevel;
        ClanTag = clanTag;
    }

    public static Player Create(string name, string tag, int townHallLevel, string? clanTag = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new InvalidPlayerException("Player name cannot be empty.");
        if (townHallLevel < 1) throw new InvalidPlayerException("Player town hall level must be at least 1.");
        Tag tagValue = Tag.Create(tag);
        Tag? clanTagValue = clanTag is null ? null : Tag.Create(clanTag);

        Player player = new Player(name.Trim(), tagValue, townHallLevel, clanTagValue);
        player.AddDomainEvent(new PlayerCreatedEvent(player.Id, player.Name, player.Tag.Value, player.TownHallLevel, player.ClanTag?.Value));

        return player;
    }

    public static Player Restore(Guid id, string name, string tag, int townHallLevel, string? clanTag = null)
    {
        Tag? clanTagValue = clanTag is null ? null : Tag.Restore(clanTag);

        return new Player(name, Tag.Restore(tag), townHallLevel, clanTagValue, id);
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new InvalidPlayerException("Player name cannot be empty.");

        string normalizedName = name.Trim();
        if (string.Equals(Name, normalizedName, StringComparison.Ordinal)) return;

        string previousName = Name;
        Name = normalizedName;
        this.AddDomainEvent(new PlayerNameUpdatedEvent(Id, previousName, Name));
    }

    public void UpdateTownHallLevel(int townHallLevel)
    {
        if (townHallLevel < 1) throw new InvalidPlayerException("Player town hall level must be at least 1.");

        if (TownHallLevel == townHallLevel) return;

        int previousTownHallLevel = TownHallLevel;
        TownHallLevel = townHallLevel;
        this.AddDomainEvent(new PlayerTownHallLevelUpdatedEvent(Id, previousTownHallLevel, TownHallLevel));
    }

    public void UpdateClanTag(string? clanTag)
    {
        Tag? updatedClanTag = clanTag is null ? null : Tag.Create(clanTag);
        if (ClanTag == updatedClanTag) return;

        string? previousClanTag = ClanTag?.Value;
        ClanTag = updatedClanTag;
        this.AddDomainEvent(new PlayerClanUpdatedEvent(Id, previousClanTag, ClanTag?.Value));
    }
}
