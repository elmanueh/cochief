namespace Cochief.Domain.Events;

using Cochief.Domain.Shared;

public sealed class ClanCreatedEvent : DomainEvent
{
    public Guid ClanId { get; }
    public string Name { get; }
    public string Tag { get; }

    public ClanCreatedEvent(Guid clanId, string name, string tag)
    {
        ClanId = clanId;
        Name = name;
        Tag = tag;
    }
}
