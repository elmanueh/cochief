namespace Cochief.Domain.Ports;

public interface IClanService
{
    Task UpdateAllAsync(CancellationToken ct);
    Task UpdateAsync(Guid clanId, CancellationToken ct);
}
