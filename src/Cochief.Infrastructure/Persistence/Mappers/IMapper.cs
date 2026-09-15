namespace Cochief.Infrastructure.Persistence.Mappers;

internal interface IMapper<TModel, TEntity>
{
    TModel ToDomain(TEntity entity);
    TEntity ToPersistence(TModel model);
}
