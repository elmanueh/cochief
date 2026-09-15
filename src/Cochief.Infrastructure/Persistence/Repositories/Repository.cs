namespace Cochief.Infrastructure.Persistence.Repositories;

using Cochief.Domain.Ports;
using Cochief.Domain.Shared;
using Cochief.Infrastructure.Persistence.Exceptions;
using Cochief.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

internal abstract class Repository<TModel, TEntity>(CochiefDbContext dbContext, IMapper<TModel, TEntity> mapper) : IRepository<TModel>
    where TModel : class, IIdentifiable
    where TEntity : class, IIdentifiable
{
    private readonly CochiefDbContext _dbContext = dbContext;
    private readonly DbSet<TEntity> _entities = dbContext.Set<TEntity>();

    protected CochiefDbContext DbContext => _dbContext;
    protected DbSet<TEntity> Entities => _entities;
    protected IMapper<TModel, TEntity> Mapper { get; } = mapper;
    protected virtual IQueryable<TEntity> Query => Entities;

    public virtual async Task CreateAsync(TModel model, CancellationToken ct)
    {
        await Entities.AddAsync(Mapper.ToPersistence(model), ct);
    }

    public virtual async Task<TModel> GetByIdAsync(Guid id, CancellationToken ct)
    {
        TEntity? trackedEntity = Entities.Local.FirstOrDefault(entity => entity.Id == id);
        if (trackedEntity is not null) return Mapper.ToDomain(trackedEntity);

        TEntity entity = await Query.FirstOrDefaultAsync(entity => EF.Property<Guid>(entity, "Id") == id, ct)
            ?? throw new EntityNotFoundException($"{typeof(TModel).Name} '{id}' was not found.");

        return Mapper.ToDomain(entity);
    }

    public virtual async Task<IReadOnlyList<TModel>> GetAllAsync(CancellationToken ct)
    {
        TEntity[] entities = await Query.ToArrayAsync(ct);

        return entities.Select(Mapper.ToDomain).ToArray();
    }

    public virtual async Task UpdateAsync(TModel model, CancellationToken ct)
    {
        TEntity? entity = Entities.Local.FirstOrDefault(entity => entity.Id == model.Id);
        entity ??= await Query.FirstOrDefaultAsync(entity => EF.Property<Guid>(entity, "Id") == model.Id, ct);
        if (entity is null) throw new EntityNotFoundException($"{typeof(TModel).Name} '{model.Id}' was not found.");

        Apply(model, entity);
    }

    public virtual async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        TEntity? entity = Entities.Local.FirstOrDefault(entity => entity.Id == id);
        entity ??= await Query.FirstOrDefaultAsync(entity => EF.Property<Guid>(entity, "Id") == id, ct);
        if (entity is null) throw new EntityNotFoundException($"{typeof(TModel).Name} '{id}' was not found.");

        Entities.Remove(entity);
    }

    protected abstract void Apply(TModel model, TEntity entity);
}
