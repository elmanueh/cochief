namespace Cochief.Application.Services;

using Cochief.Application.Exceptions;
using Cochief.Domain.Model;
using Cochief.Domain.Ports;

public sealed class AuthService(IUserService userService, IPasswordHasher passwordHasher) : IAuthService
{
    public async Task<User> RegisterAsync(string name, string email, string password, CancellationToken ct)
    {
        User user = await userService.CreateUserAsync(name, email, password, ct);

        return user;
    }

    public async Task<User> LoginAsync(string email, string password, CancellationToken ct)
    {
        User? user = await userService.GetUserByEmailAsync(email, ct);

        if (user is null || !passwordHasher.Verify(password, user.PasswordHash))
        {
            throw new AuthException("Email or password is incorrect.");
        }

        return user;
    }
}
