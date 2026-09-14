namespace Cochief.Domain.Ports;

using Cochief.Domain.Model;
using Cochief.Domain.ValueObjects;

public interface IClanRepository : IRepository<Clan>
{
    Task<Clan?> FindByTagAsync(Tag tag, CancellationToken ct);
}
