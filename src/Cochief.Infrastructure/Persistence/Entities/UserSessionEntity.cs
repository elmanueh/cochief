namespace Cochief.Infrastructure.Persistence.Entities;

using Cochief.Domain.Shared;

internal sealed class UserSessionEntity : IIdentifiable
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string RefreshTokenHash { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
}
