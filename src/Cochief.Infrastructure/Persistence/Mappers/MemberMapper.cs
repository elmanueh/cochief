namespace Cochief.Infrastructure.Persistence.Mappers;

using Cochief.Domain.Model;
using Cochief.Infrastructure.Persistence.Entities;

internal sealed class MemberMapper : IMapper<Member, MemberEntity>
{
    public Member ToDomain(MemberEntity entity)
    {
        return Member.Restore(entity.Id, entity.PlayerId, entity.ClanId, entity.Role);
    }

    public MemberEntity ToPersistence(Member model)
    {
        return new MemberEntity
        {
            Id = model.Id,
            PlayerId = model.PlayerId,
            ClanId = model.ClanId,
            Role = model.Role
        };
    }
}
