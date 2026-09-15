namespace Cochief.Infrastructure.Persistence.Entities;

using Cochief.Domain.Shared;

internal sealed class ClanEntity : IIdentifiable
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Tag { get; set; } = null!;
    public ICollection<MemberEntity> Members { get; } = [];
}
