namespace Cochief.Infrastructure.Persistence.Entities;

using Cochief.Domain.Shared;

internal sealed class PlayerEntity : IIdentifiable
{
    public Guid Id { get; set; }
    public string Tag { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int TownHallLevel { get; set; }
    public string? ClanTag { get; set; }
}
