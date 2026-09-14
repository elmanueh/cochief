namespace Cochief.Domain.Ports;

using Cochief.Domain.Model;

public interface IUserSessionRepository : IRepository<UserSession>
{
    Task<UserSession?> FindByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken ct);
}
