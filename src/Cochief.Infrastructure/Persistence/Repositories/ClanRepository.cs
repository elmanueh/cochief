namespace Cochief.Infrastructure.Persistence.Repositories;

using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Cochief.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

public sealed class ClanRepository(CochiefDbContext dbContext) : Repository<Clan>(dbContext), IClanRepository
{
    protected override IQueryable<Clan> Query => base.Query.Include(clan => clan.Members);

    protected override IQueryable<Clan> TrackedQuery => base.TrackedQuery.Include(clan => clan.Members);

    public async Task<Clan?> FindByTagAsync(Tag tag, CancellationToken ct)
    {
        Clan? trackedClan = Entities.Local.FirstOrDefault(clan => clan.Tag == tag);
        if (trackedClan is not null) return trackedClan;

        return await Query.FirstOrDefaultAsync(clan => clan.Tag == tag, ct);
    }

    protected override Guid GetId(Clan model) => model.Id;
}
