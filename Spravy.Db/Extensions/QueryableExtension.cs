using Microsoft.EntityFrameworkCore.Query;

namespace Spravy.Db.Extensions;

public static class QueryableExtension
{
    public static ConfiguredValueTaskAwaitable<
        Result<ReadOnlyMemory<TSource>>
    > ToArrayEntitiesAsync<TSource>(this IQueryable<TSource> source, CancellationToken ct)
    {
        return source.ToArrayEntitiesCore(ct).ConfigureAwait(false);
    }

    public static Result RemoveRangeEntities<TSource>(
        this DbContext context,
        ReadOnlyMemory<TSource> items
    )
        where TSource : class
    {
        context.Set<TSource>().RemoveRange(items.ToArray());

        return Result.Success;
    }

    private static async ValueTask<Result<ReadOnlyMemory<TSource>>> ToArrayEntitiesCore<TSource>(
        this IQueryable<TSource> source,
        CancellationToken ct
    )
    {
        var array = await source.ToArrayAsync(ct);

        return array.ToReadOnlyMemory().ToResult();
    }

    public static ConfiguredValueTaskAwaitable<Result<int>> ExecuteUpdateEntityAsync<TSource>(
        this IQueryable<TSource> source,
        Expression<Func<SetPropertyCalls<TSource>, SetPropertyCalls<TSource>>> setPropertyCalls,
        CancellationToken ct
    )
    {
        return source.ExecuteUpdateEntityCore(setPropertyCalls, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result<int>> ExecuteUpdateEntityCore<TSource>(
        this IQueryable<TSource> source,
        Expression<Func<SetPropertyCalls<TSource>, SetPropertyCalls<TSource>>> setPropertyCalls,
        CancellationToken ct
    )
    {
        var result = await source.ExecuteUpdateAsync(setPropertyCalls, ct);

        return result.ToResult();
    }
}
