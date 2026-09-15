namespace Cochief.Infrastructure.Persistence.Repositories;

using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Cochief.Infrastructure.Persistence.Entities;
using Cochief.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

internal sealed class UserSessionRepository(CochiefDbContext dbContext, UserSessionMapper mapper) : Repository<UserSession, UserSessionEntity>(dbContext, mapper), IUserSessionRepository
{
    public async Task<UserSession?> FindByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken ct)
    {
        UserSessionEntity? entity = await Query.FirstOrDefaultAsync(session => session.RefreshTokenHash == refreshTokenHash, ct);

        return entity is null ? null : Mapper.ToDomain(entity);
    }

    protected override void Apply(UserSession model, UserSessionEntity entity)
    {
        entity.UserId = model.UserId;
        entity.RefreshTokenHash = model.RefreshTokenHash;
        entity.CreatedAt = model.CreatedAt;
        entity.ExpiresAt = model.ExpiresAt;
        entity.RevokedAt = model.RevokedAt;
    }
}
