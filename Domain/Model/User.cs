using Cochief.Domain.Exceptions;
using Cochief.Domain.ValueObjects;

namespace Cochief.Domain.Model;

public sealed class User
{
    public Guid Id { get; }
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public Player? Player { get; private set; }

    private User(Guid id, string name, Email email, string passwordHash)
    {
        Id = id;
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Player = null;
    }

    public static User Create(string name, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new InvalidUserException("User name cannot be empty.");
        Email mail = Email.Create(email);
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new InvalidUserException("User password cannot be empty.");

        return new User(Guid.NewGuid(), name.Trim(), mail, passwordHash);
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
