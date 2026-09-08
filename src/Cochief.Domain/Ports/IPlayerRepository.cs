namespace Cochief.Domain.Ports;

using Cochief.Domain.Model;
using Cochief.Domain.ValueObjects;

public interface IPlayerRepository : IRepository<Player>
{
    Task<Player?> FindByTagAsync(Tag tag, CancellationToken ct);
}
