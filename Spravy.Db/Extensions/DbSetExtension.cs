using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;

namespace Spravy.Db.Extensions;

public static class DbSetExtension
{
    public static ConfiguredValueTaskAwaitable<Result<EntityEntry<TEntity>>> AddEntityAsync<TEntity>(
        this DbSet<TEntity> dbSet,
        TEntity entity,
        CancellationToken cancellationToken
    ) where TEntity : class
    {
        return AddEntityCore(dbSet, entity, cancellationToken).ConfigureAwait(false);
    }

    private static async ValueTask<Result<EntityEntry<TEntity>>> AddEntityCore<TEntity>(
        this DbSet<TEntity> dbSet,
        TEntity entity,
        CancellationToken cancellationToken
    ) where TEntity : class
    {
        var value = await dbSet.AddAsync(entity, cancellationToken);

        return value.ToResult();
    }
}