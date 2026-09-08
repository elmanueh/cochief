namespace Cochief.Infrastructure.Persistence.Repositories;

using Cochief.Domain.Ports;

public sealed class UnitOfWork(CochiefDbContext dbContext) : IUnitOfWork
{
    private readonly CochiefDbContext _dbContext = dbContext;

    public Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return _dbContext.SaveChangesAsync(ct);
    }
}
