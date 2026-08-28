using Cochief.Domain.Exceptions;
using Cochief.Domain.ValueObjects;

namespace Cochief.Domain.Model;

public sealed class User
{
    private Guid Id { get; }
    private string Name { get; set; }
    private EmailAddress Email { get; set; }
    private string Password { get; set; }
    private Player? Player { get; set; }

    private User(Guid id, string name, EmailAddress email, string password)
    {
        Id = id;
        Name = name;
        Email = email;
        Password = password;
        Player = null;
    }

    public static User Create(string name, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new InvalidUserException("User name cannot be empty.");
        EmailAddress emailAddress = EmailAddress.Create(email);
        if (string.IsNullOrWhiteSpace(password)) throw new InvalidUserException("User password cannot be empty.");

        return new User(Guid.NewGuid(), name.Trim(), emailAddress, password);
    }

    public void LinkPlayer(Player player)
    {
        if (player is null) throw new InvalidUserException("The player linked to the user cannot be null.");
        if (Player is not null) throw new InvalidUserException("User already has a linked player.");

        Player = player;
    }

    public void UnlinkPlayer()
    {
        if (Player is null) throw new InvalidUserException("User does not have a linked player.");

        Player = null;
    }
}
