using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using AutoMapper;
using Spravy.Authentication.Db.Contexts;
using Spravy.Authentication.Db.Extensions;
using Spravy.Authentication.Db.Models;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Service.Interfaces;
using Spravy.Db.Extensions;
using Spravy.Domain.Enums;
using Spravy.Domain.Errors;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Service.Extensions;

namespace Spravy.Authentication.Service.Services;

public class EfAuthenticationService : IAuthenticationService
{
    private readonly SpravyDbAuthenticationDbContext context;
    private readonly IHasher hasher;
    private readonly IFactory<string, IHasher> hasherFactory;
    private readonly ITokenFactory tokenFactory;
    private readonly IMapper mapper;
    private readonly IPasswordValidator passwordValidator;
    private readonly ILoginValidator loginValidator;
    private readonly IEmailService emailService;
    private readonly IRandom<string> randomString;

    public EfAuthenticationService(
        SpravyDbAuthenticationDbContext context,
        IHasher hasher,
        IFactory<string, IHasher> hasherFactory,
        ITokenFactory tokenFactory,
        IMapper mapper,
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
        this.mapper = mapper;
        this.loginValidator = loginValidator;
        this.passwordValidator = passwordValidator;
        this.emailService = emailService;
        this.randomString = randomString;
    }

    public ConfiguredValueTaskAwaitable<Result<TokenResult>> LoginAsync(User user, CancellationToken cancellationToken)
    {
        return context.GetVerifiedUserByLoginAsync(user.Login, cancellationToken)
            .IfSuccessAsync(
                userEntity => CheckPassword(user.Password, userEntity)
                    .IfSuccess(() => tokenFactory.Create(mapper.Map<UserTokenClaims>(userEntity))), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> CreateUserAsync(
        CreateUserOptions options,
        CancellationToken cancellationToken
    )
    {
        return CreateUserCore(options, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> CreateUserCore(
        CreateUserOptions options,
        CancellationToken cancellationToken
    )
    {
        var email = options.Email.Trim().ToUpperInvariant();

        await foreach (var error in loginValidator.ValidateAsync(options.Login, nameof(options.Login))
                           .WithCancellation(cancellationToken))
        {
            if (error.IsHasError)
            {
                return error;
            }
        }

        await foreach (var error in passwordValidator.ValidateAsync(options.Password, nameof(options.Password))
                           .WithCancellation(cancellationToken))
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
            Email = email
        };

        return await context.AtomicExecuteAsync(
            () =>
                context.GetUserByLoginAsync(options.Login, cancellationToken)
                    .IfSuccessAsync(_ => new Result(new UserWithLoginExistsError(options.Login)), cancellationToken)
                    .IfErrorsAsync(errors => errors.GetSingle("Errors").IfSuccess(error =>
                    {
                        if (error.Id == UserWithLoginNotExistsError.MainId)
                        {
                            return Result.Success;
                        }

                        return new Result(error);
                    }))
                    .IfSuccessAsync(
                        () => context.GetUserByEmailAsync(options.Email, cancellationToken)
                            .IfSuccessAsync(_ => new Result(new UserWithEmailExistsError(options.Email)),
                                cancellationToken)
                            .IfErrorsAsync(errors => errors.GetSingle("Errors").IfSuccess(error =>
                            {
                                if (error.Id == UserWithEmailNotExistsError.MainId)
                                {
                                    return Result.Success;
                                }

                                return new Result(error);
                            })),
                        cancellationToken)
                    .IfSuccessAsync(() => context.AddEntityAsync(newUser, cancellationToken), cancellationToken)
                    .IfSuccessAsync(_ => Result.Success, cancellationToken),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result<TokenResult>> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken
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

                return context.FindEntityAsync<UserEntity>(id).IfSuccessAsync(
                    userEntity => tokenFactory.Create(mapper.Map<UserTokenClaims>(userEntity)), cancellationToken);
            }
            case Role.Service:
            {
                return tokenFactory.Create().ToValueTaskResult().ConfigureAwait(false);
            }
            default:
                return new Result<TokenResult>(new RoleOutOfRangeError(role)).ToValueTaskResult().ConfigureAwait(false);
        }
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateVerificationCodeByLoginAsync(
        string login,
        CancellationToken cancellationToken
    )
    {
        login = login.Trim();

        return context.AtomicExecuteAsync(() => context.GetUserByEmailAsync(login, cancellationToken).IfSuccessAsync(
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
                    cancellationToken
                );
            }, cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateVerificationCodeByEmailAsync(
        string email,
        CancellationToken cancellationToken
    )
    {
        email = email.Trim().ToUpperInvariant();

        return context.AtomicExecuteAsync(
            () => context.GetUserByEmailAsync(email, cancellationToken).IfSuccessAsync(userEntity =>
            {
                var verificationCode = randomString.GetRandom().ThrowIfNull();
                var hash = hasher.ComputeHash(verificationCode);
                userEntity.VerificationCodeMethod = hasher.HashMethod;
                userEntity.VerificationCodeHash = hash;

                return emailService.SendEmailAsync("VerificationCode", email, verificationCode, cancellationToken);
            }, cancellationToken),
            cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<bool>> IsVerifiedByLoginAsync(
        string login,
        CancellationToken cancellationToken
    )
    {
        login = login.Trim();

        return context.GetUserByLoginAsync(login, cancellationToken)
            .IfSuccessAsync(user => user.IsEmailVerified.ToResult(), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<bool>> IsVerifiedByEmailAsync(
        string email,
        CancellationToken cancellationToken
    )
    {
        email = email.Trim().ToUpperInvariant();

        return context.GetUserByEmailAsync(email, cancellationToken)
            .IfSuccessAsync(userEntity => userEntity.IsEmailVerified.ToResult(), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> VerifiedEmailByLoginAsync(
        string login,
        string verificationCode,
        CancellationToken cancellationToken
    )
    {
        login = login.Trim();

        return context.AtomicExecuteAsync(() => context.GetUserByLoginAsync(login, cancellationToken).IfSuccessAsync(
            userEntity => CheckVerificationCode(verificationCode, userEntity).IfSuccess(() =>
            {
                userEntity.IsEmailVerified = true;
                userEntity.VerificationCodeMethod = null;
                userEntity.VerificationCodeHash = null;

                return Result.Success;
            }), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> VerifiedEmailByEmailAsync(
        string email,
        string verificationCode,
        CancellationToken cancellationToken
    )
    {
        email = email.Trim().ToUpperInvariant();

        return context.AtomicExecuteAsync(() =>
            context.GetUserByEmailAsync(email, cancellationToken)
                .IfSuccessAsync(userEntity => CheckVerificationCode(verificationCode, userEntity).IfSuccess(() =>
                {
                    userEntity.IsEmailVerified = true;
                    userEntity.VerificationCodeMethod = null;
                    userEntity.VerificationCodeHash = null;

                    return Result.Success;
                }), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateEmailNotVerifiedUserByEmailAsync(
        string email,
        string newEmail,
        CancellationToken cancellationToken
    )
    {
        email = email.Trim().ToUpperInvariant();

        return context.AtomicExecuteAsync(() => context.GetNotVerifiedUserByEmailAsync(email, cancellationToken)
            .IfSuccessAsync(
                userEntity =>
                {
                    userEntity.Email = newEmail;

                    return Result.Success;
                }, cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateEmailNotVerifiedUserByLoginAsync(
        string login,
        string newEmail,
        CancellationToken cancellationToken
    )
    {
        login = login.Trim();

        return context.AtomicExecuteAsync(() => context.GetNotVerifiedUserByLoginAsync(login, cancellationToken)
            .IfSuccessAsync(
                userEntity =>
                {
                    userEntity.Email = newEmail;

                    return Result.Success;
                }, cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> DeleteUserByEmailAsync(
        string email,
        string verificationCode,
        CancellationToken cancellationToken
    )
    {
        email = email.Trim().ToUpperInvariant();

        return context.AtomicExecuteAsync(() =>
            context.GetVerifiedUserByEmailAsync(email, cancellationToken)
                .IfSuccessAsync(
                    userEntity => CheckVerificationCode(verificationCode, userEntity)
                        .IfSuccess(() => context.RemoveEntity(userEntity)), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> DeleteUserByLoginAsync(
        string login,
        string verificationCode,
        CancellationToken cancellationToken
    )
    {
        login = login.Trim();

        return context.AtomicExecuteAsync(() =>
            context.GetVerifiedUserByLoginAsync(login, cancellationToken).IfSuccessAsync(
                userEntity => CheckVerificationCode(verificationCode, userEntity)
                    .IfSuccess(() => context.RemoveEntity(userEntity)), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordByEmailAsync(
        string email,
        string verificationCode,
        string newPassword,
        CancellationToken cancellationToken
    )
    {
        email = email.Trim().ToUpperInvariant();

        return context.AtomicExecuteAsync(() => context.GetVerifiedUserByEmailAsync(email, cancellationToken)
            .IfSuccessAsync(
                userEntity =>
                    CheckVerificationCode(verificationCode, userEntity).IfSuccess(() => hasherFactory
                        .Create(userEntity.HashMethod.ThrowIfNullOrWhiteSpace()).IfSuccess(newHasher =>
                        {
                            var hash = newHasher.ComputeHash($"{userEntity.Salt};{newPassword}");
                            userEntity.VerificationCodeMethod = null;
                            userEntity.VerificationCodeHash = null;
                            userEntity.HashMethod = newHasher.HashMethod;
                            userEntity.PasswordHash = hash;

                            return Result.Success;
                        }))
                , cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordByLoginAsync(
        string login,
        string verificationCode,
        string newPassword,
        CancellationToken cancellationToken
    )
    {
        login = login.Trim();

        return context.AtomicExecuteAsync(() =>
            context.GetUserByLoginAsync(login, cancellationToken)
                .IfSuccessAsync(userEntity => CheckVerificationCode(verificationCode, userEntity).IfSuccess(() =>
                    hasherFactory.Create(userEntity.HashMethod.ThrowIfNullOrWhiteSpace())
                        .IfSuccess(newHasher =>
                        {
                            var hash = newHasher.ComputeHash($"{userEntity.Salt};{newPassword}");
                            userEntity.VerificationCodeMethod = null;
                            userEntity.VerificationCodeHash = null;
                            userEntity.HashMethod = newHasher.HashMethod;
                            userEntity.PasswordHash = hash;

                            return Result.Success;
                        })), cancellationToken), cancellationToken);
    }

    private Result Check(string code, string hashMethod, string valueHash)
    {
        return hasherFactory.Create(hashMethod.ThrowIfNullOrWhiteSpace()).IfSuccess(newHasher =>
        {
            var hash = newHasher.ComputeHash(code);

            if (hash != valueHash)
            {
                return new Result(new WrongPassword());
            }

            return Result.Success;
        });
    }

    private Result CheckVerificationCode(string code, UserEntity entity)
    {
        return Check(
            code,
            entity.VerificationCodeMethod.ThrowIfNullOrWhiteSpace(),
            entity.VerificationCodeHash.ThrowIfNullOrWhiteSpace()
        );
    }

    private Result CheckPassword(string password, UserEntity entity)
    {
        return Check(
            $"{entity.Salt};{password}",
            entity.HashMethod.ThrowIfNullOrWhiteSpace(),
            entity.PasswordHash.ThrowIfNullOrWhiteSpace()
        );
    }
}