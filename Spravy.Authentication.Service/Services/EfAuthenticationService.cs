using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Spravy.Authentication.Db.Contexts;
using Spravy.Authentication.Db.Models;
using Spravy.Authentication.Domain.Exceptions;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Service.Interfaces;
using Spravy.Db.Extensions;
using Spravy.Domain.Enums;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Domain.ValidationResults;
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

    public async Task<Result<TokenResult>> LoginAsync(User user, CancellationToken cancellationToken)
    {
        var userEntity = await context.Set<UserEntity>()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Login == user.Login && x.IsEmailVerified, cancellationToken);

        if (userEntity is null)
        {
            return new Result<TokenResult>(new UserWithLoginExistsError(user.Login));
        }

        CheckPassword(user.Password, userEntity);
        var userTokenClaims = mapper.Map<UserTokenClaims>(userEntity);
        var tokenResult = tokenFactory.Create(userTokenClaims);

        return new Result<TokenResult>(tokenResult);
    }

    public async Task<Result> CreateUserAsync(
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
            Email = email,
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

    public async Task<Result<TokenResult>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
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

    public async Task<Result> UpdateVerificationCodeByLoginAsync(string login, CancellationToken cancellationToken)
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

    public async Task<Result> UpdateVerificationCodeByEmailAsync(string email, CancellationToken cancellationToken)
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

    public async Task<Result<bool>> IsVerifiedByLoginAsync(string login, CancellationToken cancellationToken)
    {
        login = login.Trim();

        var userEntity = await context.Set<UserEntity>()
            .AsNoTracking()
            .SingleAsync(x => x.Login == login, cancellationToken);

        return new Result<bool>(userEntity.IsEmailVerified);
    }

    public async Task<Result<bool>> IsVerifiedByEmailAsync(string email, CancellationToken cancellationToken)
    {
        email = email.Trim().ToUpperInvariant();

        var userEntity = await context.Set<UserEntity>()
            .AsNoTracking()
            .SingleAsync(x => x.Email == email, cancellationToken);

        return new Result<bool>(userEntity.IsEmailVerified);
    }

    public async Task<Result> VerifiedEmailByLoginAsync(
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

    public async Task<Result> VerifiedEmailByEmailAsync(
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

    public async Task<Result> UpdateEmailNotVerifiedUserByEmailAsync(
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

    public async Task<Result> UpdateEmailNotVerifiedUserByLoginAsync(
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

    public async Task<Result> DeleteUserByEmailAsync(
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

    public async Task<Result> DeleteUserByLoginAsync(
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

    public async Task<Result> UpdatePasswordByEmailAsync(
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

    public async Task<Result> UpdatePasswordByLoginAsync(
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

    private void Check(string code, string hashMethod, string valueHash)
    {
        var newHasher = hasherFactory.Create(hashMethod.ThrowIfNullOrWhiteSpace());
        var hash = newHasher.ComputeHash(code);

        if (hash != valueHash)
        {
            throw new Exception();
        }
    }

    private void CheckVerificationCode(string code, UserEntity entity)
    {
        Check(
            code,
            entity.VerificationCodeMethod.ThrowIfNullOrWhiteSpace(),
            entity.VerificationCodeHash.ThrowIfNullOrWhiteSpace()
        );
    }

    private void CheckPassword(string password, UserEntity entity)
    {
        Check(
            $"{entity.Salt};{password}",
            entity.HashMethod.ThrowIfNullOrWhiteSpace(),
            entity.PasswordHash.ThrowIfNullOrWhiteSpace()
        );
    }
}