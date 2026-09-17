namespace Cochief.Application.IntegrationTests.Fixtures;

using Cochief.Domain.Enums;
using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Cochief.Domain.ValueObjects;

internal sealed class TestClashOfClansService : IClashOfClansService
{
    internal bool VerificationResult { get; set; }
    internal Player? Player { get; set; }
    internal Clan? Clan { get; set; }
    internal IReadOnlyDictionary<Player, MemberRole> ClanMembers { get; set; } = new Dictionary<Player, MemberRole>();

    public Task<Clan> GetClanAsync(Tag clanTag, CancellationToken ct)
    {
        return Task.FromResult(Clan ?? throw new InvalidOperationException("No test clan has been configured."));
    }

    public Task<IReadOnlyDictionary<Player, MemberRole>> GetClanMembersAsync(Tag clanTag, CancellationToken ct)
    {
        return Task.FromResult(ClanMembers);
    }

    public Task<Player> GetPlayerAsync(Tag playerTag, CancellationToken ct)
    {
        return Task.FromResult(Player ?? throw new InvalidOperationException("No test player has been configured."));
    }

    public Task<bool> VerifyPlayerTokenAsync(Tag playerTag, string token, CancellationToken ct)
    {
        return Task.FromResult(VerificationResult);
    }

    internal void Reset()
    {
        VerificationResult = false;
        Player = null;
        Clan = null;
        ClanMembers = new Dictionary<Player, MemberRole>();
    }
}
