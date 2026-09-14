namespace Cochief.Domain.Ports;

using Cochief.Domain.Model;

public interface IAuthService
{
    Task<User> RegisterAsync(string name, string email, string password, CancellationToken ct);

    Task<UserAuthentication> LoginAsync(string email, string password, CancellationToken ct);

    Task<UserAuthentication> RefreshAsync(string refreshToken, CancellationToken ct);

    Task LogoutAsync(string? refreshToken, CancellationToken ct);
}
