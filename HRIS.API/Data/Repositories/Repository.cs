using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Data.Repositories;

public class Repository<TEntity>(HrisDbContext context) : IRepository<TEntity> where TEntity : class
{
    protected readonly HrisDbContext Context = context;
    private DbSet<TEntity> Set => Context.Set<TEntity>();

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await Set.FindAsync([id], ct);

    public async Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken ct = default)
        => await Set.ToListAsync(ct);

    public async Task<IReadOnlyCollection<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        => await Set.Where(predicate).ToListAsync(ct);

    public async Task AddAsync(TEntity entity, CancellationToken ct = default)
        => await Set.AddAsync(entity, ct);

    public void Update(TEntity entity)
        => Set.Update(entity); 

    public void Remove(TEntity entity)
        => Set.Remove(entity);
}