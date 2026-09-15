namespace Cochief.Infrastructure.Persistence.Repositories;

using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Cochief.Domain.ValueObjects;
using Cochief.Infrastructure.Persistence.Entities;
using Cochief.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

internal sealed class UserRepository(CochiefDbContext dbContext, UserMapper mapper) : Repository<User, UserEntity>(dbContext, mapper), IUserRepository
{
    protected override IQueryable<UserEntity> Query => base.Query.Include(user => user.Player);

    public async Task<User?> FindByEmailAsync(Email email, CancellationToken ct)
    {
        UserEntity? entity = await Query.FirstOrDefaultAsync(user => user.Email == email.Value, ct);

        return entity is null ? null : Mapper.ToDomain(entity);
    }

    protected override void Apply(User model, UserEntity entity)
    {
        entity.Name = model.Name;
        entity.Email = model.Email.Value;
        entity.PasswordHash = model.PasswordHash;

        if (model.Player is null)
        {
            entity.Player = null;
            entity.PlayerId = null;
            return;
        }

        entity.PlayerId = model.Player.Id;
        PlayerEntity? trackedPlayer = DbContext.Players.Local.FirstOrDefault(player => player.Id == model.Player.Id);
        if (trackedPlayer is not null)
        {
            entity.Player = trackedPlayer;
        }
    }
}
