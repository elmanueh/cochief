using Cochief.Domain.Model;
using Cochief.Domain.Services;

namespace Cochief.Application.Services;

public sealed class UserService(IPasswordHasher passwordHasher) : IUserService
{
    public User CreateUser(string name, string email, string password)
    {
        string passwordHash = passwordHasher.Hash(password);

        User user = User.Create(name, email, passwordHash);

        return user;
    }

    public User GetUser(Guid userId)
    {
        User user = User.Create("change", "change@change.com", "change");

        // TODO: Retrieve the user from the database using the userId

        return user;
    }

    public async Task<User> LinkPlayerAsync(Guid userId, string playerTag, string token)
    {
        User user = GetUser(userId);

        // TODO: Add clash verification service to verify the player tag and token

        Player player = Player.Create("change", "#000000", 1);

        user.LinkPlayer(player);

        // TODO: Persist the user and player to the database

        return user;
    }

    public void UnlinkPlayer(Guid userId)
    {
        User user = GetUser(userId);

        user.UnlinkPlayer();

        // TODO: Persist the user to the database
    }

    public User GetUserByEmail(string email)
    {
        // TODO: Retrieve the user from the database using the email
        return User.Create("change", "change@change.com", "change");
    }
}
