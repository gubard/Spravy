namespace Spravy.Db.Extensions;

public static class DbContextExtension
{
    public static Result RemoveEntity<TEntity>(this DbContext context, TEntity entity) where TEntity : class
    {
        context.Remove(entity);

        return Result.Success;
    }

    public static ConfiguredValueTaskAwaitable<Result<EntityEntry<TEntity>>> AddEntityAsync<TEntity>(
        this DbContext context,
        TEntity entity,
        CancellationToken ct
    ) where TEntity : class
    {
        return AddEntityCore(context, entity, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result<EntityEntry<TEntity>>> AddEntityCore<TEntity>(
        this DbContext context,
        TEntity entity,
        CancellationToken ct
    ) where TEntity : class
    {
        var value = await context.AddAsync(entity, ct);

        return value.ToResult();
    }

    public static ConfiguredValueTaskAwaitable<Result<TEntity>> GetEntityAsync<TEntity>(
        this DbContext context,
        object key
    ) where TEntity : class
    {
        return GetEntityCore<TEntity>(context, key).ConfigureAwait(false);
    }

    private static async ValueTask<Result<TEntity>> GetEntityCore<TEntity>(this DbContext context, object key)
        where TEntity : class
    {
        var value = await context.FindAsync<TEntity>(key);

        if (value is null)
        {
            return new(new NotFoundEntityError(typeof(TEntity).Name, key.ToString().ThrowIfNull()));
        }

        return value.ToResult();
    }

    public static ConfiguredValueTaskAwaitable<Result<Option<TEntity>>> FindEntityAsync<TEntity>(
        this DbContext context,
        object key
    ) where TEntity : class
    {
        return FindEntityCore<TEntity>(context, key).ConfigureAwait(false);
    }

    private static async ValueTask<Result<Option<TEntity>>> FindEntityCore<TEntity>(this DbContext context, object key)
        where TEntity : class
    {
        var value = await context.FindAsync<TEntity>(key);

        return value.ToOption().ToResult();
    }

    public static ConfiguredValueTaskAwaitable<Result<TReturn>> AtomicExecuteAsync<TDbContext, TReturn>(
        this TDbContext context,
        Func<ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        CancellationToken ct
    ) where TDbContext : DbContext where TReturn : notnull
    {
        return AtomicExecuteCore(context, func, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result<TReturn>> AtomicExecuteCore<TDbContext, TReturn>(
        this TDbContext context,
        Func<ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        CancellationToken ct
    ) where TDbContext : DbContext where TReturn : notnull
    {
        await using var transaction = await context.Database.BeginTransactionAsync(ct);
        Result<TReturn> result;

        try
        {
            result = await func.Invoke();
        }
        catch
        {
            await transaction.RollbackAsync(ct);

            throw;
        }

        if (result.IsHasError)
        {
            await transaction.RollbackAsync(ct);
        }
        else
        {
            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }

        return result;
    }

    public static Cvtar AtomicExecuteAsync<TDbContext>(this TDbContext context, Func<Cvtar> func, CancellationToken ct)
        where TDbContext : DbContext
    {
        return AtomicExecuteCore(context, func, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result> AtomicExecuteCore<TDbContext>(
        this TDbContext context,
        Func<Cvtar> func,
        CancellationToken ct
    ) where TDbContext : DbContext
    {
        await using var transaction = await context.Database.BeginTransactionAsync(ct);
        Result result;

        try
        {
            result = await func.Invoke();
        }
        catch
        {
            await transaction.RollbackAsync(ct);

            throw;
        }

        if (result.IsHasError)
        {
            await transaction.RollbackAsync(ct);
        }
        else
        {
            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }

        return result;
    }
}