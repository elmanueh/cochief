namespace Cochief.Domain.Events;

using Cochief.Domain.Shared;

public sealed class UserSessionCreatedEvent : DomainEvent
{
    public Guid SessionId { get; }
    public Guid UserId { get; }
    public DateTimeOffset CreatedAt { get; }

    public UserSessionCreatedEvent(Guid sessionId, Guid userId, DateTimeOffset createdAt)
    {
        SessionId = sessionId;
        UserId = userId;
        CreatedAt = createdAt;
    }
}
