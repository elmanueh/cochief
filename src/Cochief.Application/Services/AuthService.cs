namespace Cochief.Application.Services;

using Cochief.Application.Exceptions;
using Cochief.Domain.Model;
using Cochief.Domain.Ports;

public sealed class AuthService(IUserService userService, IPasswordHasher passwordHasher) : IAuthService
{
    private readonly IUserService _userService = userService;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<User> RegisterAsync(string name, string email, string password, CancellationToken ct)
    {
        User user = await _userService.CreateUserAsync(name, email, password, ct);

        return user;
    }

    public async Task<User> LoginAsync(string email, string password, CancellationToken ct)
    {
        User? user = await _userService.GetUserByEmailAsync(email, ct);

        if (user is null || !_passwordHasher.Verify(password, user.PasswordHash))
        {
            throw new AuthException("Email or password is incorrect.");
        }

        return user;
    }
}
