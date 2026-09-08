using Cochief.Domain.Exceptions;
using Cochief.Domain.Events;
using Cochief.Domain.Shared;
using Cochief.Domain.ValueObjects;

namespace Cochief.Domain.Model;

public sealed class User : AggregateRoot
{
    public string Name { get; }
    public Email Email { get; }
    public string PasswordHash { get; }
    public Player? Player { get; private set; }

    private User(string name, Email email, string passwordHash, Guid? id = null) : base(id)
    {
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

        User user = new User(name.Trim(), mail, passwordHash);
        user.AddDomainEvent(new UserCreatedEvent(user.Id, user.Name, user.Email.Value));

        return user;
    }

    public static User Restore(Guid id, string name, string email, string passwordHash, Player? player = null)
    {
        User user = new User(name, Email.Restore(email), passwordHash, id);
        user.Player = player;

        return user;
    }

    public void LinkPlayer(Player player)
    {
        if (player is null) throw new InvalidUserException("The player linked to the user cannot be null.");
        if (Player is not null) throw new InvalidUserException("User already has a linked player.");

        Player = player;
        this.AddDomainEvent(new UserPlayerLinkedEvent(Id, player.Id));
    }

    public void UnlinkPlayer()
    {
        Player player = Player ?? throw new InvalidUserException("User does not have a linked player.");

        Player = null;
        this.AddDomainEvent(new UserPlayerUnlinkedEvent(Id, player.Id));
    }
}
