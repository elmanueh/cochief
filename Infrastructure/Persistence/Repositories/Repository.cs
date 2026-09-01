namespace Cochief.Infrastructure.Persistence.Repositories;

using Cochief.Domain.Ports;
using Cochief.Infrastructure.Persistence.Exceptions;
using Microsoft.EntityFrameworkCore;

public abstract class Repository<TModel>(CochiefDbContext dbContext) : IRepository<TModel>
    where TModel : class
{
    protected CochiefDbContext DbContext { get; } = dbContext;
    protected DbSet<TModel> Entities { get; } = dbContext.Set<TModel>();
    protected virtual IQueryable<TModel> Query => Entities;
    protected virtual IQueryable<TModel> TrackedQuery => Entities;

    public virtual async Task CreateAsync(TModel model, CancellationToken ct)
    {
        await Entities.AddAsync(model, ct);
    }

    public virtual async Task<TModel> GetByIdAsync(Guid id, CancellationToken ct)
    {
        TModel model = await Query.FirstOrDefaultAsync(entity => EF.Property<Guid>(entity, "Id") == id, ct)
            ?? throw new EntityNotFoundException($"{typeof(TModel).Name} '{id}' was not found.");

        return model;
    }

    public virtual async Task<IReadOnlyList<TModel>> GetAllAsync(CancellationToken ct)
    {
        return await Query.ToListAsync(ct);
    }

    public virtual async Task UpdateAsync(TModel model, CancellationToken ct)
    {
        Guid id = GetId(model);

        TModel persistedModel = await TrackedQuery.FirstOrDefaultAsync(entity => EF.Property<Guid>(entity, "Id") == id, ct)
            ?? throw new EntityNotFoundException($"{typeof(TModel).Name} '{id}' was not found.");

        Apply(model, persistedModel);
    }

    public virtual async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        TModel model = await TrackedQuery.FirstOrDefaultAsync(entity => EF.Property<Guid>(entity, "Id") == id, ct)
            ?? throw new EntityNotFoundException($"{typeof(TModel).Name} '{id}' was not found.");

        Entities.Remove(model);
    }

    protected abstract Guid GetId(TModel model);

    protected virtual void Apply(TModel source, TModel target)
    {
        if (!ReferenceEquals(source, target))
        {
            DbContext.Entry(target).CurrentValues.SetValues(source);
        }
    }
}
