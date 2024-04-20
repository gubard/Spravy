using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Spravy.Authentication.Db.Contexts;
using Spravy.Authentication.Db.Models;
using Spravy.Domain.Errors;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;

namespace Spravy.Authentication.Db.Extensions;

public static class SpravyDbAuthenticationDbContextExtension
{

    public static ConfiguredValueTaskAwaitable<Result<UserEntity>> GetVerifiedUserByLoginAsync(
        this SpravyDbAuthenticationDbContext context,
        string login,
        CancellationToken cancellationToken
    )
    {
        return GetUserByLoginCore(context, login, cancellationToken).ConfigureAwait(false);
    }

    private static async ValueTask<Result<UserEntity>> GetVerifiedUserByLoginCore(
        this SpravyDbAuthenticationDbContext context,
        string login,
        CancellationToken cancellationToken
    )
    {
        var users = await context.Set<UserEntity>().Where(x => x.Login == login).ToArrayAsync(cancellationToken);

        if (users.Length == 0)
        {
            return new Result<UserEntity>(new UserWithLoginExistsError(login));
        }

        if (users.Length > 1)
        {
            return new Result<UserEntity>(new MultiUsersWithSameLoginError(login));
        }

        if (!users[0].IsEmailVerified)
        {
            return new Result<UserEntity>(new UserNotVerifiedError(
                users[0].Login.ThrowIfNullOrWhiteSpace(),
                users[0].Email.ThrowIfNullOrWhiteSpace()
            ));
        }

        return new Result<UserEntity>(users[0]);
    }

    public static ConfiguredValueTaskAwaitable<Result<UserEntity>> GetUserByLoginAsync(
        this SpravyDbAuthenticationDbContext context,
        string login,
        CancellationToken cancellationToken
    )
    {
        return GetUserByLoginCore(context, login, cancellationToken).ConfigureAwait(false);
    }

    private static async ValueTask<Result<UserEntity>> GetUserByLoginCore(
        this SpravyDbAuthenticationDbContext context,
        string login,
        CancellationToken cancellationToken
    )
    {
        var users = await context.Set<UserEntity>().Where(x => x.Login == login).ToArrayAsync(cancellationToken);

        if (users.Length == 0)
        {
            return new Result<UserEntity>(new UserWithLoginExistsError(login));
        }

        if (users.Length > 1)
        {
            return new Result<UserEntity>(new MultiUsersWithSameLoginError(login));
        }

        return new Result<UserEntity>(users[0]);
    }
}