namespace Spravy.Db.Extensions;

public static class DbSetExtension
{
    public static ConfiguredValueTaskAwaitable<
        Result<EntityEntry<TEntity>>
    > AddEntityAsync<TEntity>(this DbSet<TEntity> dbSet, TEntity entity, CancellationToken ct)
        where TEntity : class
    {
        return AddEntityCore(dbSet, entity, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result<EntityEntry<TEntity>>> AddEntityCore<TEntity>(
        this DbSet<TEntity> dbSet,
        TEntity entity,
        CancellationToken ct
    )
        where TEntity : class
    {
        var value = await dbSet.AddAsync(entity, ct);

        return value.ToResult();
    }
}
