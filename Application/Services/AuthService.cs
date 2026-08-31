using Cochief.Application.Exceptions;
using Cochief.Domain.Model;
using Cochief.Domain.Services;

namespace Cochief.Application.Services;

public sealed class AuthService(IUserService userService, IPasswordHasher passwordHasher) : IAuthService
{
    public User Register(string name, string email, string password)
    {
        User user = userService.CreateUser(name, email, password);

        return user;
    }

    public User Login(string email, string password)
    {
        User user = userService.GetUserByEmail(email);

        bool isPasswordValid = passwordHasher.Verify(password, user.PasswordHash);

        if (!isPasswordValid) throw new AuthException("Email or password is incorrect.");

        return user;
    }
}
