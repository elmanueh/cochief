namespace Cochief.Infrastructure.Persistence.Repositories;

using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Microsoft.EntityFrameworkCore;

public sealed class UserSessionRepository(CochiefDbContext dbContext) : Repository<UserSession>(dbContext), IUserSessionRepository
{
    public async Task<UserSession?> FindByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken ct)
    {
        return await Query.FirstOrDefaultAsync(session => session.RefreshTokenHash == refreshTokenHash, ct);
    }

    protected override Guid GetId(UserSession model) => model.Id;
}
