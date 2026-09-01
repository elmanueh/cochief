using Cochief.Domain.Exceptions;
using Cochief.Domain.ValueObjects;

namespace Cochief.Domain.Model;

public sealed class Player
{
    public Guid Id { get; }
    public Tag Tag { get; }
    public string Name { get; private set; }
    public int TownHallLevel { get; private set; }
    public Guid? ClanId { get; private set; }

    private Player(Guid id, string name, Tag tag, int townHallLevel, Guid? clanId = null)
    {
        Id = id;
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

        return new Player(Guid.NewGuid(), name.Trim(), tagValue, townHallLevel, clanId);
    }

    public static Player Restore(Guid id, string name, string tag, int townHallLevel, Guid? clanId = null)
    {
        return new Player(id, name, Tag.Restore(tag), townHallLevel, clanId);
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new InvalidPlayerException("Player name cannot be empty.");

        Name = name.Trim();
    }

    public void UpdateTownHallLevel(int townHallLevel)
    {
        if (townHallLevel < 1) throw new InvalidPlayerException("Player town hall level must be at least 1.");

        TownHallLevel = townHallLevel;
    }

    public void UpdateClanId(Guid? clanId)
    {
        ClanId = clanId;
    }
}
