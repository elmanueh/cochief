namespace Cochief.Domain.Model;

using Cochief.Domain.Events;
using Cochief.Domain.Exceptions;
using Cochief.Domain.Shared;

public sealed class UserSession : AggregateRoot
{
    public Guid UserId { get; }
    public string RefreshTokenHash { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }

    private UserSession(Guid userId, string refreshTokenHash, DateTimeOffset createdAt, DateTimeOffset expiresAt, DateTimeOffset? revokedAt = null, Guid? id = null) : base(id)
    {
        UserId = userId;
        RefreshTokenHash = refreshTokenHash;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
        RevokedAt = revokedAt;
    }

    public static UserSession Create(Guid userId, string refreshTokenHash, DateTimeOffset createdAt, DateTimeOffset expiresAt, Guid? id = null)
    {
        if (userId == Guid.Empty) throw new InvalidUserSessionException("Session user cannot be empty.");
        if (string.IsNullOrWhiteSpace(refreshTokenHash))
            throw new InvalidUserSessionException("Refresh token hash cannot be empty.");
        if (expiresAt <= createdAt)
            throw new InvalidUserSessionException("Refresh token expiration must be in the future.");

        UserSession session = new UserSession(userId, refreshTokenHash, createdAt, expiresAt, id: id);
        session.AddDomainEvent(new UserSessionCreatedEvent(session.Id, session.UserId, session.CreatedAt));

        return session;
    }

    public static UserSession Restore(Guid id, Guid userId, string refreshTokenHash, DateTimeOffset createdAt, DateTimeOffset expiresAt, DateTimeOffset? revokedAt)
    {
        return new UserSession(userId, refreshTokenHash, createdAt, expiresAt, revokedAt, id);
    }

    public bool IsActive(DateTimeOffset now)
    {
        return RevokedAt is null && ExpiresAt > now;
    }

    public void Rotate(string refreshTokenHash, DateTimeOffset expiresAt, DateTimeOffset occurredAt)
    {
        if (!IsActive(occurredAt)) throw new InvalidUserSessionException("The session is not active.");
        if (string.IsNullOrWhiteSpace(refreshTokenHash))
            throw new InvalidUserSessionException("Refresh token hash cannot be empty.");
        if (expiresAt <= occurredAt)
            throw new InvalidUserSessionException("Refresh token expiration must be in the future.");

        RefreshTokenHash = refreshTokenHash;
        ExpiresAt = expiresAt;
        this.AddDomainEvent(new UserSessionRefreshedEvent(Id, UserId, occurredAt));
    }

    public void Revoke(DateTimeOffset occurredAt)
    {
        if (RevokedAt is not null) return;

        RevokedAt = occurredAt;
        this.AddDomainEvent(new UserSessionRevokedEvent(Id, UserId, occurredAt));
    }
}
