using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;

namespace Spravy.Db.Extensions;

public static class QueryableExtension
{
    public static ConfiguredValueTaskAwaitable<Result<TEntity>> SingleOrDefaultEntityAsync<TEntity>(
        this IQueryable<TEntity> context,
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken
    )
        where TEntity : class
    {
        return SingleOrDefaultEntityCore(context, predicate, cancellationToken)
            .ConfigureAwait(false);
    }

    private static async ValueTask<Result<TEntity>> SingleOrDefaultEntityCore<TEntity>(
        this IQueryable<TEntity> context,
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken
    )
        where TEntity : class
    {
        var value = await context.SingleOrDefaultAsync(predicate, cancellationToken);

        return value.ToResult();
    }

    public static ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TSource>>> ToArrayEntitiesAsync<TSource>(
        this IQueryable<TSource> source,
        CancellationToken cancellationToken)
    {
        return source.ToArrayEntitiesCore(cancellationToken).ConfigureAwait(false);
    }

    private static async ValueTask<Result<ReadOnlyMemory<TSource>>> ToArrayEntitiesCore<TSource>(
        this IQueryable<TSource> source,
        CancellationToken cancellationToken)
    {
        var array = await source.ToArrayAsync(cancellationToken);

        return array.ToReadOnlyMemory().ToResult();
    }
}