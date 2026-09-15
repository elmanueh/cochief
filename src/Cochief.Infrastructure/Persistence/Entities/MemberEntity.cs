namespace Cochief.Infrastructure.Persistence.Entities;

using Cochief.Domain.Enums;
using Cochief.Domain.Shared;

internal sealed class MemberEntity : IIdentifiable
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public Guid ClanId { get; set; }
    public MemberRole Role { get; set; }
}
