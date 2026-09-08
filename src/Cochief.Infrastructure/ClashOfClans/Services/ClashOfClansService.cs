namespace Cochief.Infrastructure.ClashOfClans.Services;

using Cochief.Domain.Enums;
using Cochief.Domain.Ports;
using Cochief.Domain.ValueObjects;
using Cochief.Infrastructure.ClashOfClans.Generated;
using Cochief.Infrastructure.ClashOfClans.Mappers;

internal sealed class ClashOfClansService(IClashOfClansApiClient apiClient) : IClashOfClansService
{
    private readonly IClashOfClansApiClient _apiClient = apiClient;

    public async Task<IReadOnlyDictionary<Domain.Model.Player, MemberRole>> GetClanMembersAsync(Tag clanTag, CancellationToken ct)
    {
        ICollection<ClanMember> clanMembers = await _apiClient.GetClanMembersAsync(clanTag.Value, 50, null, null, ct);
        Dictionary<Domain.Model.Player, MemberRole> members = [];

        foreach (ClanMember clanMember in clanMembers)
        {
            Domain.Model.Player player = Domain.Model.Player.Create(
                clanMember.Name ?? string.Empty,
                clanMember.Tag ?? string.Empty,
                clanMember.TownHallLevel ?? 0);

            members.Add(player, MemberRoleMapper.Map(clanMember.Role));
        }

        return members;
    }

    public async Task<Domain.Model.Player> GetPlayerAsync(Tag playerTag, CancellationToken ct)
    {
        Player playerCoc = await _apiClient.GetPlayerAsync(playerTag.Value, ct);

        string name = string.IsNullOrWhiteSpace(playerCoc.Name) ? "" : playerCoc.Name;
        string tag = string.IsNullOrWhiteSpace(playerCoc.Tag) ? "" : playerCoc.Tag;
        int townHallLevel = playerCoc.TownHallLevel ?? 0;

        Domain.Model.Player player = Domain.Model.Player.Create(name, tag, townHallLevel);

        return player;
    }

    public async Task<bool> VerifyPlayerTokenAsync(Tag playerTag, string token, CancellationToken ct)
    {
        VerifyTokenResponse response = await _apiClient.VerifyTokenAsync(playerTag.Value, new VerifyTokenRequest { Token = token }, ct);

        return string.Equals(response.Status, "ok", StringComparison.OrdinalIgnoreCase);
    }
}
