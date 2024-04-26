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
    public static ConfiguredValueTaskAwaitable<Result<UserEntity>> GetNotVerifiedUserByEmailAsync(
        this SpravyDbAuthenticationDbContext context,
        string email,
        CancellationToken cancellationToken
    )
    {
        return context.GetUserByEmailAsync(email, cancellationToken).IfSuccessAsync(user =>
        {
            if (user.IsEmailVerified)
            {
                return Result.Success.IfSuccess(
                    user.Login.CheckNullOrWhiteSpaceProperty(nameof(user.Login)),
                    user.Email.CheckNullOrWhiteSpaceProperty(nameof(user.Email)),
                    (l, e) => new Result<UserEntity>(new UserVerifiedError(l, e))
                );
            }

            return new Result<UserEntity>(user);
        }, cancellationToken);
    }

    public static ConfiguredValueTaskAwaitable<Result<UserEntity>> GetNotVerifiedUserByLoginAsync(
        this SpravyDbAuthenticationDbContext context,
        string login,
        CancellationToken cancellationToken
    )
    {
        return context.GetUserByLoginAsync(login, cancellationToken).IfSuccessAsync(user =>
        {
            if (!user.IsEmailVerified)
            {
                return Result.Success.IfSuccess(
                    user.Login.CheckNullOrWhiteSpaceProperty(nameof(user.Login)),
                    user.Email.CheckNullOrWhiteSpaceProperty(nameof(user.Email)),
                    (l, e) => new Result<UserEntity>(new UserVerifiedError(l, e))
                );
            }

            return new Result<UserEntity>(user);
        }, cancellationToken);
    }
    
    public static ConfiguredValueTaskAwaitable<Result<UserEntity>> GetVerifiedUserByEmailAsync(
        this SpravyDbAuthenticationDbContext context,
        string email,
        CancellationToken cancellationToken
    )
    {
        return context.GetUserByEmailAsync(email, cancellationToken).IfSuccessAsync(user =>
        {
            if (!user.IsEmailVerified)
            {
                return Result.Success.IfSuccess(
                    user.Login.CheckNullOrWhiteSpaceProperty(nameof(user.Login)),
                    user.Email.CheckNullOrWhiteSpaceProperty(nameof(user.Email)),
                    (l, e) => new Result<UserEntity>(new UserNotVerifiedError(l, e))
                );
            }

            return new Result<UserEntity>(user);
        }, cancellationToken);
    }

    public static ConfiguredValueTaskAwaitable<Result<UserEntity>> GetVerifiedUserByLoginAsync(
        this SpravyDbAuthenticationDbContext context,
        string login,
        CancellationToken cancellationToken
    )
    {
        return context.GetUserByLoginAsync(login, cancellationToken).IfSuccessAsync(user =>
        {
            if (!user.IsEmailVerified)
            {
                return Result.Success.IfSuccess(
                    user.Login.CheckNullOrWhiteSpaceProperty(nameof(user.Login)),
                    user.Email.CheckNullOrWhiteSpaceProperty(nameof(user.Email)),
                    (l, e) => new Result<UserEntity>(new UserNotVerifiedError(l, e))
                );
            }

            return new Result<UserEntity>(user);
        }, cancellationToken);
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

    public static ConfiguredValueTaskAwaitable<Result<UserEntity>> GetUserByEmailAsync(
        this SpravyDbAuthenticationDbContext context,
        string email,
        CancellationToken cancellationToken
    )
    {
        return GetUserByEmailCore(context, email, cancellationToken).ConfigureAwait(false);
    }

    private static async ValueTask<Result<UserEntity>> GetUserByEmailCore(
        this SpravyDbAuthenticationDbContext context,
        string email,
        CancellationToken cancellationToken
    )
    {
        var users = await context.Set<UserEntity>().Where(x => x.Email == email).ToArrayAsync(cancellationToken);

        if (users.Length == 0)
        {
            return new Result<UserEntity>(new UserWithEmailNotExistsError(email));
        }

        if (users.Length > 1)
        {
            return new Result<UserEntity>(new MultiUsersWithSameEmailError(email));
        }

        return new Result<UserEntity>(users[0]);
    }
}