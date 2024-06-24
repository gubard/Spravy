namespace Spravy.Db.Extensions;

public static class QueryableExtension
{
    public static ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TSource>>> ToArrayEntitiesAsync<TSource>(
        this IQueryable<TSource> source,
        CancellationToken ct
    )
    {
        return source.ToArrayEntitiesCore(ct).ConfigureAwait(false);
    }
    
    private static async ValueTask<Result<ReadOnlyMemory<TSource>>> ToArrayEntitiesCore<TSource>(
        this IQueryable<TSource> source,
        CancellationToken ct
    )
    {
        var array = await source.ToArrayAsync(ct);
        
        return array.ToReadOnlyMemory().ToResult();
    }
}