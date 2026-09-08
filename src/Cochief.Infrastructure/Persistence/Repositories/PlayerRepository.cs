namespace Cochief.Infrastructure.Persistence.Repositories;

using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Cochief.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

public sealed class PlayerRepository(CochiefDbContext dbContext) : Repository<Player>(dbContext), IPlayerRepository
{
    public async Task<Player?> FindByTagAsync(Tag tag, CancellationToken ct)
    {
        return await Query.SingleOrDefaultAsync(player => player.Tag == tag, ct);
    }

    protected override Guid GetId(Player model) => model.Id;
}
