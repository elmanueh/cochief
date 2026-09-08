namespace Cochief.Domain.Events;

using Cochief.Domain.Shared;

public sealed class PlayerTownHallLevelUpdatedEvent : DomainEvent
{
    public Guid PlayerId { get; }
    public int PreviousTownHallLevel { get; }
    public int TownHallLevel { get; }

    public PlayerTownHallLevelUpdatedEvent(Guid playerId, int previousTownHallLevel, int townHallLevel)
    {
        PlayerId = playerId;
        PreviousTownHallLevel = previousTownHallLevel;
        TownHallLevel = townHallLevel;
    }
}
