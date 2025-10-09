namespace Spravy.Authentication.Db.Extensions;

public static class SpravyDbAuthenticationDbContextExtension
{
    public static ConfiguredValueTaskAwaitable<Result<UserEntity>> GetNotVerifiedUserByEmailAsync(
        this AuthenticationSpravyDbContext context,
        string email,
        CancellationToken ct
    )
    {
        return context.GetUserByEmailAsync(email, ct)
           .IfSuccessAsync(
                user =>
                {
                    if (user.IsEmailVerified)
                    {
                        return user.Login
                           .CheckNullOrWhiteSpaceProperty(nameof(user.Login))
                           .IfSuccess(login =>
                                user.Email
                                   .CheckNullOrWhiteSpaceProperty(nameof(user.Email))
                                   .IfSuccess(e => new Result<UserEntity>(new UserVerifiedError(login, e)))
                            );
                    }

                    return new(user);
                },
                ct
            );
    }

    public static ConfiguredValueTaskAwaitable<Result<UserEntity>> GetNotVerifiedUserByLoginAsync(
        this AuthenticationSpravyDbContext context,
        string login,
        CancellationToken ct
    )
    {
        return context.GetUserByLoginAsync(login, ct)
           .IfSuccessAsync(
                user =>
                {
                    if (!user.IsEmailVerified)
                    {
                        return user.Login
                           .CheckNullOrWhiteSpaceProperty(nameof(user.Login))
                           .IfSuccess(l =>
                                user.Email
                                   .CheckNullOrWhiteSpaceProperty(nameof(user.Email))
                                   .IfSuccess(email => new Result<UserEntity>(new UserVerifiedError(l, email)))
                            );
                    }

                    return new(user);
                },
                ct
            );
    }

    public static ConfiguredValueTaskAwaitable<Result<UserEntity>> GetVerifiedUserByEmailAsync(
        this AuthenticationSpravyDbContext context,
        string email,
        CancellationToken ct
    )
    {
        return context.GetUserByEmailAsync(email, ct)
           .IfSuccessAsync(
                user =>
                {
                    if (!user.IsEmailVerified)
                    {
                        return user.Login
                           .CheckNullOrWhiteSpaceProperty(nameof(user.Login))
                           .IfSuccess(login =>
                                user.Email
                                   .CheckNullOrWhiteSpaceProperty(nameof(user.Email))
                                   .IfSuccess(e => new Result<UserEntity>(new UserVerifiedError(login, e)))
                            );
                    }

                    return new(user);
                },
                ct
            );
    }

    public static ConfiguredValueTaskAwaitable<Result<UserEntity>> GetVerifiedUserByLoginAsync(
        this AuthenticationSpravyDbContext context,
        string login,
        CancellationToken ct
    )
    {
        return context.GetUserByLoginAsync(login, ct)
           .IfSuccessAsync(
                user =>
                {
                    if (!user.IsEmailVerified)
                    {
                        return user.Login
                           .CheckNullOrWhiteSpaceProperty(nameof(user.Login))
                           .IfSuccess(l =>
                                user.Email
                                   .CheckNullOrWhiteSpaceProperty(nameof(user.Email))
                                   .IfSuccess(email => new Result<UserEntity>(new UserVerifiedError(l, email)))
                            );
                    }

                    return new(user);
                },
                ct
            );
    }

    public static ConfiguredValueTaskAwaitable<Result<UserEntity>> GetUserByLoginAsync(
        this AuthenticationSpravyDbContext context,
        string login,
        CancellationToken ct
    )
    {
        return GetUserByLoginCore(context, login, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result<UserEntity>> GetUserByLoginCore(
        this AuthenticationSpravyDbContext context,
        string login,
        CancellationToken ct
    )
    {
        var users = await context.Set<UserEntity>().Where(x => x.Login == login).ToArrayAsync(ct);

        if (users.Length == 0)
        {
            return new(new UserWithLoginNotExistsError(login));
        }

        if (users.Length > 1)
        {
            return new(new MultiUsersWithSameLoginError(login));
        }

        return new(users[0]);
    }

    public static ConfiguredValueTaskAwaitable<Result<UserEntity>> GetUserByEmailAsync(
        this AuthenticationSpravyDbContext context,
        string email,
        CancellationToken ct
    )
    {
        return GetUserByEmailCore(context, email, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result<UserEntity>> GetUserByEmailCore(
        this AuthenticationSpravyDbContext context,
        string email,
        CancellationToken ct
    )
    {
        var users = await context.Set<UserEntity>().Where(x => x.Email == email).ToArrayAsync(ct);

        if (users.Length == 0)
        {
            return new(new UserWithEmailNotExistsError(email));
        }

        if (users.Length > 1)
        {
            return new(new MultiUsersWithSameEmailError(email));
        }

        return new(users[0]);
    }
}