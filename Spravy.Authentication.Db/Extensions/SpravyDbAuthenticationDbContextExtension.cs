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
        var user = await context.GetUserByLoginAsync(login, cancellationToken);

        if (user.IsHasError)
        {
            return new Result<UserEntity>(user.Errors);
        }

        if (!user.Value.IsEmailVerified)
        {
            return new Result<UserEntity>(new UserNotVerifiedError(
                user.Value.Login.ThrowIfNullOrWhiteSpace(),
                user.Value.Email.ThrowIfNullOrWhiteSpace()
            ));
        }

        return new Result<UserEntity>(user.Value);
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
            return new Result<UserEntity>(new UserWithLoginNotExistsError(login));
        }

        if (users.Length > 1)
        {
            return new Result<UserEntity>(new MultiUsersWithSameLoginError(login));
        }

        return new Result<UserEntity>(users[0]);
    }
}