namespace Spravy.Db.Extensions;

public static class QueryableExtension
{
    public static ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TSource>>> ToArrayEntitiesAsync<TSource>(
        this IQueryable<TSource> source,
        CancellationToken cancellationToken
    )
    {
        return source.ToArrayEntitiesCore(cancellationToken).ConfigureAwait(false);
    }
    
    private static async ValueTask<Result<ReadOnlyMemory<TSource>>> ToArrayEntitiesCore<TSource>(
        this IQueryable<TSource> source,
        CancellationToken cancellationToken
    )
    {
        var array = await source.ToArrayAsync(cancellationToken);
        
        return array.ToReadOnlyMemory().ToResult();
    }
}