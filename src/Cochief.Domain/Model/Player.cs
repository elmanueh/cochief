using Cochief.Domain.Exceptions;
using Cochief.Domain.Events;
using Cochief.Domain.Shared;
using Cochief.Domain.ValueObjects;

namespace Cochief.Domain.Model;

public sealed class Player : AggregateRoot
{
    public Tag Tag { get; }
    public string Name { get; private set; }
    public int TownHallLevel { get; private set; }
    public Guid? ClanId { get; private set; }

    private Player(string name, Tag tag, int townHallLevel, Guid? clanId = null, Guid? id = null) : base(id)
    {
        Name = name;
        Tag = tag;
        TownHallLevel = townHallLevel;
        ClanId = clanId;
    }

    public static Player Create(string name, string tag, int townHallLevel, Guid? clanId = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new InvalidPlayerException("Player name cannot be empty.");
        Tag tagValue = Tag.Create(tag);
        if (townHallLevel < 1) throw new InvalidPlayerException("Player town hall level must be at least 1.");

        Player player = new Player(name.Trim(), tagValue, townHallLevel, clanId);
        player.AddDomainEvent(new PlayerCreatedEvent(player.Id, player.Name, player.Tag.Value, player.TownHallLevel, player.ClanId));

        return player;
    }

    public static Player Restore(Guid id, string name, string tag, int townHallLevel, Guid? clanId = null)
    {
        return new Player(name, Tag.Restore(tag), townHallLevel, clanId, id);
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

    public void UpdateClanId(Guid? clanId)
    {
        if (ClanId == clanId) return;

        Guid? previousClanId = ClanId;
        ClanId = clanId;
        this.AddDomainEvent(new PlayerClanUpdatedEvent(Id, previousClanId, ClanId));
    }
}
