namespace Cochief.Infrastructure.Persistence.Repositories;

using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Cochief.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

public sealed class UserRepository(CochiefDbContext dbContext) : Repository<User>(dbContext), IUserRepository
{
    protected override IQueryable<User> Query => base.Query.Include(user => user.Player);

    protected override IQueryable<User> TrackedQuery => base.TrackedQuery.Include(user => user.Player);

    public async Task<User?> FindByEmailAsync(Email email, CancellationToken ct)
    {
        return await Query.FirstOrDefaultAsync(user => user.Email == email, ct);
    }

    protected override Guid GetId(User model) => model.Id;

    protected override void Apply(User source, User target)
    {
        base.Apply(source, target);

        if (ReferenceEquals(source, target))
        {
            return;
        }

        if (source.Player is null)
        {
            DbContext.Entry(target).Reference(user => user.Player).CurrentValue = null;
            return;
        }

        if (target.Player?.Id == source.Player.Id)
        {
            DbContext.Entry(target.Player).CurrentValues.SetValues(source.Player);
            return;
        }

        DbContext.Entry(target).Reference(user => user.Player).CurrentValue = source.Player;
    }
}
