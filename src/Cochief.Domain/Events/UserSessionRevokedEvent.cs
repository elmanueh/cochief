namespace Cochief.Domain.Events;

using Cochief.Domain.Shared;

public sealed class UserSessionRevokedEvent : DomainEvent
{
    public Guid SessionId { get; }
    public Guid UserId { get; }
    public DateTimeOffset RevokedAt { get; }

    public UserSessionRevokedEvent(Guid sessionId, Guid userId, DateTimeOffset revokedAt)
    {
        SessionId = sessionId;
        UserId = userId;
        RevokedAt = revokedAt;
    }
}
