using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Spravy.Domain.Errors;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.PasswordGenerator.Db.Errors;
using Spravy.PasswordGenerator.Db.Models;

namespace Spravy.PasswordGenerator.Db.Extensions;

public static class UserSecretEntityExtension
{
    public static ConfiguredValueTaskAwaitable<Result<UserSecretEntity>> GetSecretByUserIdAsync(
        this IQueryable<UserSecretEntity> queryable,
        Guid userId,
        CancellationToken ct
    )
    {
        return GetSecretByUserIdCore(queryable, userId, ct).ConfigureAwait(false);
    }
    
    private static async ValueTask<Result<UserSecretEntity>> GetSecretByUserIdCore(
        IQueryable<UserSecretEntity> queryable,
        Guid userId,
        CancellationToken ct
    )
    {
        var items = await queryable.Where(x => x.UserId == userId).Take(2).ToArrayAsync(ct);
        
        if (items.Length == 0)
        {
            return new(new NotFoundUserSecretError(userId));
        }
        
        if (items.Length == 2)
        {
            return new(new MultiValuesArrayError("UserSecrets", (ulong)items.Length));
        }
        
        return items[0].ToResult();
    }
}