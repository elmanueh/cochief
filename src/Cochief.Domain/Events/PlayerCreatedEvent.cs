namespace Cochief.Domain.Events;

using Cochief.Domain.Shared;

public sealed class PlayerCreatedEvent : DomainEvent
{
    public Guid PlayerId { get; }
    public string Name { get; }
    public string Tag { get; }
    public int TownHallLevel { get; }
    public Guid? ClanId { get; }

    public PlayerCreatedEvent(Guid playerId, string name, string tag, int townHallLevel, Guid? clanId)
    {
        PlayerId = playerId;
        Name = name;
        Tag = tag;
        TownHallLevel = townHallLevel;
        ClanId = clanId;
    }
}
