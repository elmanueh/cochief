namespace Cochief.Infrastructure.Persistence.Mappers;

using Cochief.Domain.Model;
using Cochief.Infrastructure.Persistence.Entities;

internal sealed class ClanMapper(MemberMapper memberMapper) : IMapper<Clan, ClanEntity>
{
    private readonly MemberMapper _memberMapper = memberMapper;

    public Clan ToDomain(ClanEntity entity)
    {
        Member[] members = entity.Members.Select(_memberMapper.ToDomain).ToArray();

        return Clan.Restore(entity.Id, entity.Name, entity.Tag, members);
    }

    public ClanEntity ToPersistence(Clan model)
    {
        ClanEntity entity = new ClanEntity()
        {
            Id = model.Id,
            Name = model.Name,
            Tag = model.Tag.Value
        };

        foreach (Member member in model.Members)
        {
            entity.Members.Add(_memberMapper.ToPersistence(member));
        }

        return entity;
    }
}
