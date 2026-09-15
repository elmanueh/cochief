namespace Cochief.Infrastructure.Persistence.Mappers;

using Cochief.Domain.Model;
using Cochief.Infrastructure.Persistence.Entities;

internal sealed class PlayerMapper : IMapper<Player, PlayerEntity>
{
    public Player ToDomain(PlayerEntity entity)
    {
        return Player.Restore(entity.Id, entity.Name, entity.Tag, entity.TownHallLevel, entity.ClanTag);
    }

    public PlayerEntity ToPersistence(Player model)
    {
        return new PlayerEntity
        {
            Id = model.Id,
            Tag = model.Tag.Value,
            Name = model.Name,
            TownHallLevel = model.TownHallLevel,
            ClanTag = model.ClanTag?.Value
        };
    }
}
