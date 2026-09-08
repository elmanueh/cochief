namespace Cochief.Infrastructure.Persistence.Repositories;

using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Microsoft.EntityFrameworkCore;

public sealed class ClanRepository(CochiefDbContext dbContext) : Repository<Clan>(dbContext), IClanRepository
{
    protected override IQueryable<Clan> Query => base.Query.Include(clan => clan.Members);

    protected override IQueryable<Clan> TrackedQuery => base.TrackedQuery.Include(clan => clan.Members);

    protected override Guid GetId(Clan model) => model.Id;
}
