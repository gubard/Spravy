using Spravy.Authentication.Db.Models;
using Spravy.Db.Extensions;

namespace Spravy.Authentication.Service.Services;

public class EfAuthenticationService : IAuthenticationService
{
    private readonly SpravyDbAuthenticationDbContext context;
    private readonly IEmailService emailService;
    private readonly IHasher hasher;
    private readonly IFactory<string, IHasher> hasherFactory;
    private readonly ILoginValidator loginValidator;
    private readonly IPasswordValidator passwordValidator;
    private readonly IRandom<string> randomString;
    private readonly ITokenFactory tokenFactory;

    public EfAuthenticationService(
        SpravyDbAuthenticationDbContext context,
        IHasher hasher,
        IFactory<string, IHasher> hasherFactory,
        ITokenFactory tokenFactory,
        ILoginValidator loginValidator,
        IPasswordValidator passwordValidator,
        IEmailService emailService,
        IRandom<string> randomString
    )
    {
        this.context = context;
        this.hasher = hasher;
        this.hasherFactory = hasherFactory;
        this.tokenFactory = tokenFactory;
        this.loginValidator = loginValidator;
        this.passwordValidator = passwordValidator;
        this.emailService = emailService;
        this.randomString = randomString;
    }

    public ConfiguredValueTaskAwaitable<Result<TokenResult>> LoginAsync(
        User user,
        CancellationToken ct
    )
    {
        return context
            .GetVerifiedUserByLoginAsync(user.Login, ct)
            .IfSuccessAsync(
                userEntity =>
                    CheckPassword(user.Password, userEntity)
                        .IfSuccess(() => tokenFactory.Create(userEntity.ToUserTokenClaims())),
                ct
            );
    }

    public Cvtar CreateUserAsync(CreateUserOptions options, CancellationToken ct)
    {
        return CreateUserCore(options, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<Result<TokenResult>> RefreshTokenAsync(
        string refreshToken,
        CancellationToken ct
    )
    {
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadJwtToken(refreshToken);
        var role = jwtToken.Claims.GetRoleClaim().Value.ParseEnum<Role>();

        switch (role)
        {
            case Role.User:
            {
                var id = Guid.Parse(jwtToken.Claims.GetNameIdentifierClaim().Value);

                return context
                    .GetEntityAsync<UserEntity>(id)
                    .IfSuccessAsync(
                        userEntity => tokenFactory.Create(userEntity.ToUserTokenClaims()),
                        ct
                    );
            }
            case Role.Service:
            {
                return tokenFactory.Create().ToValueTaskResult().ConfigureAwait(false);
            }
            default:
                return new Result<TokenResult>(new RoleOutOfRangeError(role))
                    .ToValueTaskResult()
                    .ConfigureAwait(false);
        }
    }

    public Cvtar UpdateVerificationCodeByLoginAsync(string login, CancellationToken ct)
    {
        login = login.Trim();

        return context.AtomicExecuteAsync(
            () =>
                context
                    .GetUserByLoginAsync(login, ct)
                    .IfSuccessAsync(
                        userEntity =>
                        {
                            var verificationCode = randomString.GetRandom().ThrowIfNull();
                            var hash = hasher.ComputeHash(verificationCode);
                            userEntity.VerificationCodeMethod = hasher.HashMethod;
                            userEntity.VerificationCodeHash = hash;

                            return emailService.SendEmailAsync(
                                "VerificationCode",
                                userEntity.Email.ThrowIfNullOrWhiteSpace(),
                                verificationCode,
                                ct
                            );
                        },
                        ct
                    ),
            ct
        );
    }

    public Cvtar UpdateVerificationCodeByEmailAsync(string email, CancellationToken ct)
    {
        email = email.Trim().ToUpperInvariant();

        return context.AtomicExecuteAsync(
            () =>
                context
                    .GetUserByEmailAsync(email, ct)
                    .IfSuccessAsync(
                        userEntity =>
                        {
                            var verificationCode = randomString.GetRandom().ThrowIfNull();
                            var hash = hasher.ComputeHash(verificationCode);
                            userEntity.VerificationCodeMethod = hasher.HashMethod;
                            userEntity.VerificationCodeHash = hash;

                            return emailService.SendEmailAsync(
                                "VerificationCode",
                                email,
                                verificationCode,
                                ct
                            );
                        },
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<bool>> IsVerifiedByLoginAsync(
        string login,
        CancellationToken ct
    )
    {
        login = login.Trim();

        return context
            .GetUserByLoginAsync(login, ct)
            .IfSuccessAsync(user => user.IsEmailVerified.ToResult(), ct);
    }

    public ConfiguredValueTaskAwaitable<Result<bool>> IsVerifiedByEmailAsync(
        string email,
        CancellationToken ct
    )
    {
        email = email.Trim().ToUpperInvariant();

        return context
            .GetUserByEmailAsync(email, ct)
            .IfSuccessAsync(userEntity => userEntity.IsEmailVerified.ToResult(), ct);
    }

    public Cvtar VerifiedEmailByLoginAsync(
        string login,
        string verificationCode,
        CancellationToken ct
    )
    {
        login = login.Trim();

        return context.AtomicExecuteAsync(
            () =>
                context
                    .GetUserByLoginAsync(login, ct)
                    .IfSuccessAsync(
                        userEntity =>
                            CheckVerificationCode(verificationCode, userEntity)
                                .IfSuccess(() =>
                                {
                                    userEntity.IsEmailVerified = true;
                                    userEntity.VerificationCodeMethod = null;
                                    userEntity.VerificationCodeHash = null;

                                    return Result.Success;
                                }),
                        ct
                    ),
            ct
        );
    }

    public Cvtar VerifiedEmailByEmailAsync(
        string email,
        string verificationCode,
        CancellationToken ct
    )
    {
        email = email.Trim().ToUpperInvariant();

        return context.AtomicExecuteAsync(
            () =>
                context
                    .GetUserByEmailAsync(email, ct)
                    .IfSuccessAsync(
                        userEntity =>
                            CheckVerificationCode(verificationCode, userEntity)
                                .IfSuccess(() =>
                                {
                                    userEntity.IsEmailVerified = true;
                                    userEntity.VerificationCodeMethod = null;
                                    userEntity.VerificationCodeHash = null;

                                    return Result.Success;
                                }),
                        ct
                    ),
            ct
        );
    }

    public Cvtar UpdateEmailNotVerifiedUserByEmailAsync(
        string email,
        string newEmail,
        CancellationToken ct
    )
    {
        email = email.Trim().ToUpperInvariant();

        return context.AtomicExecuteAsync(
            () =>
                context
                    .GetNotVerifiedUserByEmailAsync(email, ct)
                    .IfSuccessAsync(
                        userEntity =>
                        {
                            userEntity.Email = newEmail;

                            return Result.Success;
                        },
                        ct
                    ),
            ct
        );
    }

    public Cvtar UpdateEmailNotVerifiedUserByLoginAsync(
        string login,
        string newEmail,
        CancellationToken ct
    )
    {
        login = login.Trim();

        return context.AtomicExecuteAsync(
            () =>
                context
                    .GetNotVerifiedUserByLoginAsync(login, ct)
                    .IfSuccessAsync(
                        userEntity =>
                        {
                            userEntity.Email = newEmail;

                            return Result.Success;
                        },
                        ct
                    ),
            ct
        );
    }

    public Cvtar DeleteUserByEmailAsync(string email, string verificationCode, CancellationToken ct)
    {
        email = email.Trim().ToUpperInvariant();

        return context.AtomicExecuteAsync(
            () =>
                context
                    .GetVerifiedUserByEmailAsync(email, ct)
                    .IfSuccessAsync(
                        userEntity =>
                            CheckVerificationCode(verificationCode, userEntity)
                                .IfSuccess(() => context.RemoveEntity(userEntity)),
                        ct
                    ),
            ct
        );
    }

    public Cvtar DeleteUserByLoginAsync(string login, string verificationCode, CancellationToken ct)
    {
        login = login.Trim();

        return context.AtomicExecuteAsync(
            () =>
                context
                    .GetVerifiedUserByLoginAsync(login, ct)
                    .IfSuccessAsync(
                        userEntity =>
                            CheckVerificationCode(verificationCode, userEntity)
                                .IfSuccess(() => context.RemoveEntity(userEntity)),
                        ct
                    ),
            ct
        );
    }

    public Cvtar UpdatePasswordByEmailAsync(
        string email,
        string verificationCode,
        string newPassword,
        CancellationToken ct
    )
    {
        email = email.Trim().ToUpperInvariant();

        return context.AtomicExecuteAsync(
            () =>
                context
                    .GetVerifiedUserByEmailAsync(email, ct)
                    .IfSuccessAsync(
                        userEntity =>
                            CheckVerificationCode(verificationCode, userEntity)
                                .IfSuccess(
                                    () =>
                                        hasherFactory
                                            .Create(userEntity.HashMethod.ThrowIfNullOrWhiteSpace())
                                            .IfSuccess(newHasher =>
                                            {
                                                var hash = newHasher.ComputeHash(
                                                    $"{userEntity.Salt};{newPassword}"
                                                );
                                                userEntity.VerificationCodeMethod = null;
                                                userEntity.VerificationCodeHash = null;
                                                userEntity.HashMethod = newHasher.HashMethod;
                                                userEntity.PasswordHash = hash;

                                                return Result.Success;
                                            })
                                ),
                        ct
                    ),
            ct
        );
    }

    public Cvtar UpdatePasswordByLoginAsync(
        string login,
        string verificationCode,
        string newPassword,
        CancellationToken ct
    )
    {
        login = login.Trim();

        return context.AtomicExecuteAsync(
            () =>
                context
                    .GetUserByLoginAsync(login, ct)
                    .IfSuccessAsync(
                        userEntity =>
                            CheckVerificationCode(verificationCode, userEntity)
                                .IfSuccess(
                                    () =>
                                        hasherFactory
                                            .Create(userEntity.HashMethod.ThrowIfNullOrWhiteSpace())
                                            .IfSuccess(newHasher =>
                                            {
                                                var hash = newHasher.ComputeHash(
                                                    $"{userEntity.Salt};{newPassword}"
                                                );
                                                userEntity.VerificationCodeMethod = null;
                                                userEntity.VerificationCodeHash = null;
                                                userEntity.HashMethod = newHasher.HashMethod;
                                                userEntity.PasswordHash = hash;

                                                return Result.Success;
                                            })
                                ),
                        ct
                    ),
            ct
        );
    }

    private async ValueTask<Result> CreateUserCore(CreateUserOptions options, CancellationToken ct)
    {
        var email = options.Email.Trim().ToUpperInvariant();

        await foreach (
            var error in loginValidator
                .ValidateAsync(options.Login, nameof(options.Login))
                .WithCancellation(ct)
        )
        {
            if (error.IsHasError)
            {
                return error;
            }
        }

        await foreach (
            var error in passwordValidator
                .ValidateAsync(options.Password, nameof(options.Password))
                .WithCancellation(ct)
        )
        {
            if (error.IsHasError)
            {
                return error;
            }
        }

        var salt = Guid.NewGuid();
        var hash = hasher.ComputeHash($"{salt};{options.Password}");

        var newUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            HashMethod = hasher.HashMethod,
            Login = options.Login.Trim(),
            Salt = salt.ToString(),
            PasswordHash = hash,
            Email = email,
        };

        return await context.AtomicExecuteAsync(
            () =>
                context
                    .GetUserByLoginAsync(options.Login, ct)
                    .IfSuccessAsync(_ => new(new UserWithLoginExistsError(options.Login)), ct)
                    .IfErrorsAsync(
                        errors =>
                            errors
                                .GetSingle("Errors")
                                .IfSuccess(error =>
                                {
                                    if (error.Id == UserWithLoginNotExistsError.MainId)
                                    {
                                        return Result.Success;
                                    }

                                    return new(error);
                                }),
                        ct
                    )
                    .IfSuccessAsync(
                        () =>
                            context
                                .GetUserByEmailAsync(options.Email, ct)
                                .IfSuccessAsync(
                                    _ => new(new UserWithEmailExistsError(options.Email)),
                                    ct
                                )
                                .IfErrorsAsync(
                                    errors =>
                                        errors
                                            .GetSingle("Errors")
                                            .IfSuccess(error =>
                                            {
                                                if (error.Id == UserWithEmailNotExistsError.MainId)
                                                {
                                                    return Result.Success;
                                                }

                                                return new(error);
                                            }),
                                    ct
                                ),
                        ct
                    )
                    .IfSuccessAsync(() => context.AddEntityAsync(newUser, ct), ct)
                    .IfSuccessAsync(_ => Result.Success, ct),
            ct
        );
    }

    private Result Check(string code, string hashMethod, string valueHash)
    {
        return hasherFactory
            .Create(hashMethod.ThrowIfNullOrWhiteSpace())
            .IfSuccess(newHasher =>
            {
                var hash = newHasher.ComputeHash(code);

                if (hash != valueHash)
                {
                    return new(new WrongPasswordError());
                }

                return Result.Success;
            });
    }

    private Result CheckVerificationCode(string code, UserEntity entity)
    {
        return hasherFactory
            .Create(entity.VerificationCodeMethod.ThrowIfNullOrWhiteSpace())
            .IfSuccess(newHasher =>
            {
                var hash = newHasher.ComputeHash(code);

                if (hash != entity.VerificationCodeHash.ThrowIfNullOrWhiteSpace())
                {
                    return new(new VerificationCodePasswordError());
                }

                return Result.Success;
            });
    }

    private Result CheckPassword(string password, UserEntity entity)
    {
        return hasherFactory
            .Create(entity.HashMethod.ThrowIfNullOrWhiteSpace())
            .IfSuccess(newHasher =>
            {
                var hash = newHasher.ComputeHash($"{entity.Salt};{password}");

                if (hash != entity.PasswordHash.ThrowIfNullOrWhiteSpace())
                {
                    return new(new WrongPasswordError());
                }

                return Result.Success;
            });
    }
}
