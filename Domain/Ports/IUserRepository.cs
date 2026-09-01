namespace Cochief.Domain.Ports;

using Cochief.Domain.Model;
using Cochief.Domain.ValueObjects;

public interface IUserRepository : IRepository<User>
{
    Task<User?> FindByEmailAsync(Email email, CancellationToken ct);
}
