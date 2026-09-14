namespace Cochief.Domain.Events;

using Cochief.Domain.Shared;

public sealed class UserSessionRefreshedEvent : DomainEvent
{
    public Guid SessionId { get; }
    public Guid UserId { get; }
    public DateTimeOffset RefreshedAt { get; }

    public UserSessionRefreshedEvent(Guid sessionId, Guid userId, DateTimeOffset refreshedAt)
    {
        SessionId = sessionId;
        UserId = userId;
        RefreshedAt = refreshedAt;
    }
}
