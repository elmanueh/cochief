namespace Cochief.Infrastructure.Persistence.Repositories;

using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Cochief.Domain.ValueObjects;
using Cochief.Infrastructure.Persistence.Entities;
using Cochief.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

internal sealed class PlayerRepository(CochiefDbContext dbContext, PlayerMapper mapper) : Repository<Player, PlayerEntity>(dbContext, mapper), IPlayerRepository
{
    public async Task<Player?> FindByTagAsync(Tag tag, CancellationToken ct)
    {
        PlayerEntity? entity = await Query.SingleOrDefaultAsync(player => player.Tag == tag.Value, ct);

        return entity is null ? null : Mapper.ToDomain(entity);
    }

    protected override void Apply(Player model, PlayerEntity entity)
    {
        entity.Tag = model.Tag.Value;
        entity.Name = model.Name;
        entity.TownHallLevel = model.TownHallLevel;
        entity.ClanTag = model.ClanTag?.Value;
    }
}
