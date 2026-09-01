namespace Cochief.Domain.Ports;

using Cochief.Domain.Model;

public interface IAuthService
{
    Task<User> RegisterAsync(string name, string email, string password, CancellationToken ct);

    Task<User> LoginAsync(string email, string password, CancellationToken ct);
}
