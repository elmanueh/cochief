namespace Cochief.Domain.Ports;

public interface IClanService
{
    Task CreateAsync(string clanTag, CancellationToken ct);
    Task UpdateAllAsync(CancellationToken ct);
    Task UpdateAsync(Guid clanId, CancellationToken ct);
}
