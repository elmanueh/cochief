namespace Cochief.Domain.Ports;

using Cochief.Domain.Enums;
using Cochief.Domain.Model;
using Cochief.Domain.ValueObjects;

public interface IClashOfClansService
{
    Task<IReadOnlyDictionary<Player, MemberRole>> GetClanMembersAsync(Tag clanTag, CancellationToken ct);
    Task<Player> GetPlayerAsync(Tag playerTag, CancellationToken ct);
    Task<bool> VerifyPlayerTokenAsync(Tag playerTag, string token, CancellationToken ct);
}
