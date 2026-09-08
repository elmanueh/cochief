namespace Cochief.Domain.Events;

using Cochief.Domain.Shared;

public sealed class UserCreatedEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Name { get; }
    public string Email { get; }

    public UserCreatedEvent(Guid userId, string name, string email)
    {
        UserId = userId;
        Name = name;
        Email = email;
    }
}
