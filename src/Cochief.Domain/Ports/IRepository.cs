namespace Cochief.Domain.Ports;

public interface IRepository<TModel> where TModel : class
{
    Task CreateAsync(TModel model, CancellationToken ct);
    Task<TModel> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<TModel>> GetAllAsync(CancellationToken ct);
    Task UpdateAsync(TModel model, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}
