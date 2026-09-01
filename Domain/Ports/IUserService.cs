namespace Cochief.Domain.Ports;

using Cochief.Domain.Model;

public interface IUserService
{
    Task<User> CreateUserAsync(string name, string email, string password, CancellationToken ct);

    Task<User> GetUserAsync(Guid userId, CancellationToken ct);

    Task<User> GetUserByEmailAsync(string email, CancellationToken ct);

    Task LinkPlayerAsync(Guid userId, string playerTag, string token, CancellationToken ct);

    Task UnlinkPlayerAsync(Guid userId, CancellationToken ct);
}
