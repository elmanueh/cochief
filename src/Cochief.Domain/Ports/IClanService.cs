namespace Cochief.Domain.Ports;

using Cochief.Domain.Model;

public interface IClanService
{
    Task<IReadOnlyList<Clan>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<Clan> GetByTagAsync(Guid userId, string clanTag, CancellationToken ct);
    Task CreateAsync(string clanTag, CancellationToken ct);
    Task UpdateAllAsync(CancellationToken ct);
    Task UpdateAsync(Guid clanId, CancellationToken ct);
}
