namespace Cochief.Infrastructure.Persistence.Repositories;

using Cochief.Domain.Ports;

public sealed class UnitOfWork(CochiefDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return dbContext.SaveChangesAsync(ct);
    }
}
