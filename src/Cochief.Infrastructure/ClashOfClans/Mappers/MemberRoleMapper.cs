namespace Cochief.Infrastructure.ClashOfClans.Mappers;

using Cochief.Domain.Enums;
using Cochief.Infrastructure.ClashOfClans.Generated;

internal static class MemberRoleMapper
{
    public static MemberRole Map(ClanMemberRole? role) => role switch
    {
        ClanMemberRole.LEADER => MemberRole.Leader,
        ClanMemberRole.COLEADER => MemberRole.CoLeader,
        ClanMemberRole.ADMIN => MemberRole.Elder,
        _ => MemberRole.Member
    };
}
