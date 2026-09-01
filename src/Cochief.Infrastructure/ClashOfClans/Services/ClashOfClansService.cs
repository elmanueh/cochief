namespace Cochief.Infrastructure.ClashOfClans.Services;

using Cochief.Domain.ValueObjects;
using Cochief.Infrastructure.ClashOfClans.Contracts;
using Cochief.Infrastructure.ClashOfClans.Generated;

internal sealed class ClashOfClansService : IClashOfClansService
{
    private readonly IClashOfClansApiClient _apiClient;

    public ClashOfClansService(IClashOfClansApiClient apiClient)
    {
        _apiClient = apiClient;
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
