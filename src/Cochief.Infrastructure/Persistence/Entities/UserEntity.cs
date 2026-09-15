namespace Cochief.Infrastructure.Persistence.Entities;

using Cochief.Domain.Shared;

internal sealed class UserEntity : IIdentifiable
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public Guid? PlayerId { get; set; }
    public PlayerEntity? Player { get; set; }
}
