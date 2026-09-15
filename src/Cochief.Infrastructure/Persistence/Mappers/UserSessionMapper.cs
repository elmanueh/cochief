namespace Cochief.Infrastructure.Persistence.Mappers;

using Cochief.Domain.Model;
using Cochief.Infrastructure.Persistence.Entities;

internal sealed class UserSessionMapper : IMapper<UserSession, UserSessionEntity>
{
    public UserSession ToDomain(UserSessionEntity entity)
    {
        return UserSession.Restore(entity.Id, entity.UserId, entity.RefreshTokenHash, entity.CreatedAt, entity.ExpiresAt, entity.RevokedAt);
    }

    public UserSessionEntity ToPersistence(UserSession model)
    {
        return new UserSessionEntity
        {
            Id = model.Id,
            UserId = model.UserId,
            RefreshTokenHash = model.RefreshTokenHash,
            CreatedAt = model.CreatedAt,
            ExpiresAt = model.ExpiresAt,
            RevokedAt = model.RevokedAt
        };
    }
}
