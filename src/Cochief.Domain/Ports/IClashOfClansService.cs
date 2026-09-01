namespace Cochief.Infrastructure.ClashOfClans.Contracts;

using Cochief.Domain.Model;
using Cochief.Domain.ValueObjects;

public interface IClashOfClansService
{
    Task<Player> GetPlayerAsync(Tag playerTag, CancellationToken ct);
    Task<bool> VerifyPlayerTokenAsync(Tag playerTag, string token, CancellationToken ct);
}
