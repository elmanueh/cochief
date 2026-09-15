namespace Cochief.Infrastructure.Persistence.Repositories;

using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Cochief.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

public sealed class ClanRepository(CochiefDbContext dbContext) : Repository<Clan>(dbContext), IClanRepository
{
    protected override IQueryable<Clan> Query => base.Query
        .Include(clan => clan.Members)
        .ThenInclude(member => member.Player);

    protected override IQueryable<Clan> TrackedQuery => base.TrackedQuery
        .Include(clan => clan.Members)
        .ThenInclude(member => member.Player);

    public async Task<Clan?> FindByTagAsync(Tag tag, CancellationToken ct)
    {
        return await Query.FirstOrDefaultAsync(clan => clan.Tag == tag, ct);
    }

    protected override Guid GetId(Clan model) => model.Id;
}
