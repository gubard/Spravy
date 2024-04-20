using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        return LoginCore(user, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result<TokenResult>> LoginCore(User user, CancellationToken cancellationToken)
    {
        var userEntity = await context.GetVerifiedUserByLoginAsync(user.Login, cancellationToken);

        if (userEntity.IsHasError)
        {
            return new Result<TokenResult>(userEntity.Errors);
        }

        var check = CheckPassword(user.Password, userEntity.Value);

        if (check.IsHasError)
        {
            return new Result<TokenResult>(check.Errors);
        }

        var userTokenClaims = mapper.Map<UserTokenClaims>(userEntity.Value);
        var tokenResult = tokenFactory.Create(userTokenClaims);

        return new Result<TokenResult>(tokenResult);
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

        await foreach (var error in loginValidator.ValidateAsync(options.Login).WithCancellation(cancellationToken))
        {
            return new Result(error);
        }

        await foreach (var error in passwordValidator.ValidateAsync(options.Password)
                           .WithCancellation(cancellationToken))
        {
            return new Result(error);
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

        return await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var user = await c.Set<UserEntity>()
                    .SingleOrDefaultAsync(x => x.Login == options.Login, cancellationToken);

                if (user is not null)
                {
                    return new Result(new UserWithLoginExistsError(options.Login));
                }

                user = await c.Set<UserEntity>()
                    .SingleOrDefaultAsync(x => x.Email == email, cancellationToken);

                if (user is not null)
                {
                    return new Result(new UserWithEmailExistsError());
                }

                await c.Set<UserEntity>().AddAsync(newUser, cancellationToken);

                return Result.Success;
            },
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result<TokenResult>> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken
    )
    {
        return RefreshTokenCore(refreshToken, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result<TokenResult>> RefreshTokenCore(
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
                var loginClaim = jwtToken.Claims.GetNameClaim();

                var userEntity = await context.Set<UserEntity>()
                    .AsNoTracking()
                    .SingleAsync(x => x.Login == loginClaim.Value && x.IsEmailVerified, cancellationToken);

                var userTokenClaims = mapper.Map<UserTokenClaims>(userEntity);
                var tokenResult = tokenFactory.Create(userTokenClaims);

                return new Result<TokenResult>(tokenResult);
            }
            case Role.Service:
            {
                var tokenResult = tokenFactory.Create();

                return new Result<TokenResult>(tokenResult);
            }
            default: throw new ArgumentOutOfRangeException();
        }
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateVerificationCodeByLoginAsync(
        string login,
        CancellationToken cancellationToken
    )
    {
        return UpdateVerificationCodeByLoginCore(login, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> UpdateVerificationCodeByLoginCore(string login, CancellationToken cancellationToken)
    {
        login = login.Trim();

        var userEntity = await context.Set<UserEntity>()
            .SingleAsync(x => x.Login == login, cancellationToken);

        var verificationCode = randomString.GetRandom().ThrowIfNull();
        var hash = hasher.ComputeHash(verificationCode);
        userEntity.VerificationCodeMethod = hasher.HashMethod;
        userEntity.VerificationCodeHash = hash;
        await context.SaveChangesAsync(cancellationToken);

        await emailService.SendEmailAsync(
            "VerificationCode",
            userEntity.Email.ThrowIfNullOrWhiteSpace(),
            verificationCode,
            cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateVerificationCodeByEmailAsync(
        string email,
        CancellationToken cancellationToken
    )
    {
        return UpdateVerificationCodeByEmailCore(email, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> UpdateVerificationCodeByEmailCore(string email, CancellationToken cancellationToken)
    {
        email = email.Trim().ToUpperInvariant();

        var userEntity = await context.Set<UserEntity>()
            .SingleAsync(x => x.Email == email, cancellationToken);

        var verificationCode = randomString.GetRandom().ThrowIfNull();
        var hash = hasher.ComputeHash(verificationCode);
        userEntity.VerificationCodeMethod = hasher.HashMethod;
        userEntity.VerificationCodeHash = hash;
        await context.SaveChangesAsync(cancellationToken);
        await emailService.SendEmailAsync("VerificationCode", email, verificationCode, cancellationToken);

        return Result.Success;
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
        return IsVerifiedByEmailCore(email, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result<bool>> IsVerifiedByEmailCore(string email, CancellationToken cancellationToken)
    {
        email = email.Trim().ToUpperInvariant();

        var userEntity = await context.Set<UserEntity>()
            .AsNoTracking()
            .SingleAsync(x => x.Email == email, cancellationToken);

        return new Result<bool>(userEntity.IsEmailVerified);
    }

    public ConfiguredValueTaskAwaitable<Result> VerifiedEmailByLoginAsync(
        string login,
        string verificationCode,
        CancellationToken cancellationToken
    )
    {
        return VerifiedEmailByLoginCore(login, verificationCode, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> VerifiedEmailByLoginCore(
        string login,
        string verificationCode,
        CancellationToken cancellationToken
    )
    {
        login = login.Trim();

        var userEntity = await context.Set<UserEntity>()
            .SingleAsync(x => x.Login == login, cancellationToken);

        CheckVerificationCode(verificationCode, userEntity);
        userEntity.IsEmailVerified = true;
        userEntity.VerificationCodeMethod = null;
        userEntity.VerificationCodeHash = null;
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> VerifiedEmailByEmailAsync(
        string email,
        string verificationCode,
        CancellationToken cancellationToken
    )
    {
        return VerifiedEmailByEmailCore(email, verificationCode, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> VerifiedEmailByEmailCore(
        string email,
        string verificationCode,
        CancellationToken cancellationToken
    )
    {
        email = email.Trim().ToUpperInvariant();

        var userEntity = await context.Set<UserEntity>()
            .SingleAsync(x => x.Email == email, cancellationToken);

        CheckVerificationCode(verificationCode, userEntity);
        userEntity.IsEmailVerified = true;
        userEntity.VerificationCodeMethod = null;
        userEntity.VerificationCodeHash = null;
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateEmailNotVerifiedUserByEmailAsync(
        string email,
        string newEmail,
        CancellationToken cancellationToken
    )
    {
        return UpdateEmailNotVerifiedUserByEmailCore(email, newEmail, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Result> UpdateEmailNotVerifiedUserByEmailCore(
        string email,
        string newEmail,
        CancellationToken cancellationToken
    )
    {
        email = email.Trim().ToUpperInvariant();

        var userEntity = await context.Set<UserEntity>()
            .SingleAsync(x => x.Email == email && !x.IsEmailVerified, cancellationToken);

        userEntity.Email = newEmail;
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateEmailNotVerifiedUserByLoginAsync(
        string login,
        string newEmail,
        CancellationToken cancellationToken
    )
    {
        return UpdateEmailNotVerifiedUserByLoginCore(login, newEmail, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> UpdateEmailNotVerifiedUserByLoginCore(
        string login,
        string newEmail,
        CancellationToken cancellationToken
    )
    {
        login = login.Trim();

        var userEntity = await context.Set<UserEntity>()
            .SingleAsync(x => x.Login == login && !x.IsEmailVerified, cancellationToken);

        userEntity.Email = newEmail;
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> DeleteUserByEmailAsync(
        string email,
        string verificationCode,
        CancellationToken cancellationToken
    )
    {
        return DeleteUserByEmailCore(email, verificationCode, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> DeleteUserByEmailCore(
        string email,
        string verificationCode,
        CancellationToken cancellationToken
    )
    {
        email = email.Trim().ToUpperInvariant();

        var userEntity = await context.Set<UserEntity>()
            .SingleAsync(x => x.Email == email && x.IsEmailVerified, cancellationToken);

        CheckVerificationCode(verificationCode, userEntity);
        context.Set<UserEntity>().Remove(userEntity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> DeleteUserByLoginAsync(
        string login,
        string verificationCode,
        CancellationToken cancellationToken
    )
    {
        return DeleteUserByLoginCore(login, verificationCode, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> DeleteUserByLoginCore(
        string login,
        string verificationCode,
        CancellationToken cancellationToken
    )
    {
        login = login.Trim();

        var userEntity = await context.Set<UserEntity>()
            .SingleAsync(x => x.Login == login && x.IsEmailVerified, cancellationToken);

        CheckVerificationCode(verificationCode, userEntity);
        context.Set<UserEntity>().Remove(userEntity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordByEmailAsync(
        string email,
        string verificationCode,
        string newPassword,
        CancellationToken cancellationToken
    )
    {
        return UpdatePasswordByEmailCore(email, verificationCode, newPassword, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> UpdatePasswordByEmailCore(
        string email,
        string verificationCode,
        string newPassword,
        CancellationToken cancellationToken
    )
    {
        email = email.Trim().ToUpperInvariant();

        var userEntity = await context.Set<UserEntity>()
            .SingleAsync(x => x.Email == email && x.IsEmailVerified, cancellationToken);

        CheckVerificationCode(verificationCode, userEntity);
        var newHasher = hasherFactory.Create(userEntity.HashMethod.ThrowIfNullOrWhiteSpace());
        var hash = newHasher.ComputeHash($"{userEntity.Salt};{newPassword}");
        userEntity.VerificationCodeMethod = null;
        userEntity.VerificationCodeHash = null;
        userEntity.HashMethod = newHasher.HashMethod;
        userEntity.PasswordHash = hash;
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordByLoginAsync(
        string login,
        string verificationCode,
        string newPassword,
        CancellationToken cancellationToken
    )
    {
        return UpdatePasswordByLoginCore(login, verificationCode, newPassword, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> UpdatePasswordByLoginCore(
        string login,
        string verificationCode,
        string newPassword,
        CancellationToken cancellationToken
    )
    {
        login = login.Trim();

        var userEntity = await context.Set<UserEntity>()
            .SingleAsync(x => x.Login == login && x.IsEmailVerified, cancellationToken);

        CheckVerificationCode(verificationCode, userEntity);
        var newHasher = hasherFactory.Create(userEntity.HashMethod.ThrowIfNullOrWhiteSpace());
        var hash = newHasher.ComputeHash($"{userEntity.Salt};{newPassword}");
        userEntity.VerificationCodeMethod = null;
        userEntity.VerificationCodeHash = null;
        userEntity.HashMethod = newHasher.HashMethod;
        userEntity.PasswordHash = hash;
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }

    private Result Check(string code, string hashMethod, string valueHash)
    {
        var newHasher = hasherFactory.Create(hashMethod.ThrowIfNullOrWhiteSpace());
        var hash = newHasher.ComputeHash(code);

        if (hash != valueHash)
        {
            return new Result(new WrongPassword());
        }

        return Result.Success;
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