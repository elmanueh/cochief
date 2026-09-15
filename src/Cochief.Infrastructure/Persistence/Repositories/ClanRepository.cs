namespace Cochief.Infrastructure.Persistence.Repositories;

using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Cochief.Domain.ValueObjects;
using Cochief.Infrastructure.Persistence.Entities;
using Cochief.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

internal sealed class ClanRepository(CochiefDbContext dbContext, ClanMapper mapper, MemberMapper memberMapper) : Repository<Clan, ClanEntity>(dbContext, mapper), IClanRepository
{
    private readonly MemberMapper _memberMapper = memberMapper;

    protected override IQueryable<ClanEntity> Query => base.Query
        .Include(clan => clan.Members);

    public async Task<Clan?> FindByTagAsync(Tag tag, CancellationToken ct)
    {
        ClanEntity? entity = await Query.FirstOrDefaultAsync(clan => clan.Tag == tag.Value, ct);

        return entity is null ? null : Mapper.ToDomain(entity);
    }

    protected override void Apply(Clan model, ClanEntity entity)
    {
        entity.Name = model.Name;
        entity.Tag = model.Tag.Value;

        Dictionary<Guid, MemberEntity> persistedMembers = entity.Members.ToDictionary(member => member.Id);
        HashSet<Guid> currentMemberIds = model.Members.Select(member => member.Id).ToHashSet();

        foreach (MemberEntity removedMember in entity.Members.Where(member => !currentMemberIds.Contains(member.Id)).ToArray())
        {
            entity.Members.Remove(removedMember);
        }

        foreach (Member member in model.Members)
        {
            if (!persistedMembers.TryGetValue(member.Id, out MemberEntity? persistedMember))
            {
                entity.Members.Add(_memberMapper.ToPersistence(member));
                continue;
            }

            persistedMember.PlayerId = member.PlayerId;
            persistedMember.ClanId = member.ClanId;
            persistedMember.Role = member.Role;
        }
    }
}
