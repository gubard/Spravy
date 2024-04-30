using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Spravy.Db.Errors;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;

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
        CancellationToken cancellationToken
    ) where TEntity : class
    {
        return AddEntityCore(context, entity, cancellationToken).ConfigureAwait(false);
    }

    private static async ValueTask<Result<EntityEntry<TEntity>>> AddEntityCore<TEntity>(
        this DbContext context,
        TEntity entity,
        CancellationToken cancellationToken
    ) where TEntity : class
    {
        var value = await context.AddAsync(entity, cancellationToken);

        return value.ToResult();
    }

    public static ConfiguredValueTaskAwaitable<Result<TEntity>> FindEntityAsync<TEntity>(
        this DbContext context,
        object key
    ) where TEntity : class
    {
        return FindEntityCore<TEntity>(context, key).ConfigureAwait(false);
    }

    private static async ValueTask<Result<TEntity>> FindEntityCore<TEntity>(this DbContext context, object key)
        where TEntity : class
    {
        var value = await context.FindAsync<TEntity>(key);

        if (value is null)
        {
            return new(new NotFoundEntityError(typeof(TEntity), key));
        }

        return value.ToResult();
    }

    public static ConfiguredValueTaskAwaitable<Result<TReturn>> AtomicExecuteAsync<TDbContext, TReturn>(
        this TDbContext context,
        Func<ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        CancellationToken cancellationToken
    ) where TDbContext : DbContext
    {
        return AtomicExecuteCore(context, func, cancellationToken).ConfigureAwait(false);
    }

    private static async ValueTask<Result<TReturn>> AtomicExecuteCore<TDbContext, TReturn>(
        this TDbContext context,
        Func<ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        CancellationToken cancellationToken
    ) where TDbContext : DbContext
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        Result<TReturn> result;

        try
        {
            result = await func.Invoke();
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);

            throw;
        }

        if (result.IsHasError)
        {
            await transaction.RollbackAsync(cancellationToken);
        }
        else
        {
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        return result;
    }

    public static ConfiguredValueTaskAwaitable<Result> AtomicExecuteAsync<TDbContext>(
        this TDbContext context,
        Func<ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken cancellationToken
    ) where TDbContext : DbContext
    {
        return AtomicExecuteCore(context, func, cancellationToken).ConfigureAwait(false);
    }

    private static async ValueTask<Result> AtomicExecuteCore<TDbContext>(
        this TDbContext context,
        Func<ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken cancellationToken
    ) where TDbContext : DbContext
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        Result result;

        try
        {
            result = await func.Invoke();
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);

            throw;
        }

        if (result.IsHasError)
        {
            await transaction.RollbackAsync(cancellationToken);
        }
        else
        {
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        return result;
    }
}