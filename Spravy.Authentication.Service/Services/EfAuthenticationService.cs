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
using Spravy.Domain.Exceptions;
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
    private readonly IValidator<SpravyException, string> passwordValidator;
    private readonly IValidator<SpravyException, string> loginValidator;
    private readonly IEmailService emailService;
    private readonly IRandom<string> randomString;

    public EfAuthenticationService(
        SpravyDbAuthenticationDbContext context,
        IHasher hasher,
        IFactory<string, IHasher> hasherFactory,
        ITokenFactory tokenFactory,
        IMapper mapper,
        IValidator<SpravyException, string> loginValidator,
        IValidator<SpravyException, string> passwordValidator,
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

    public async Task<TokenResult> LoginAsync(User user, CancellationToken cancellationToken)
    {
        var userEntity = await context.Set<UserEntity>()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Login == user.Login && x.IsEmailVerified, cancellationToken);

        if (userEntity is null)
        {
            throw new UserNotFondException(user.Login);
        }

        var newHasher = hasherFactory.Create(userEntity.HashMethod.ThrowIfNullOrWhiteSpace());
        var hash = newHasher.ComputeHash($"{userEntity.Salt};{user.Password}");

        if (hash != userEntity.PasswordHash)
        {
            throw new UserWrongPasswordException(user.Login);
        }

        var userTokenClaims = mapper.Map<UserTokenClaims>(userEntity);
        var tokenResult = tokenFactory.Create(userTokenClaims);

        return tokenResult;
    }

    public Task CreateUserAsync(CreateUserOptions options, CancellationToken cancellationToken)
    {
        if (!loginValidator.Validate(options.Login, out var exception))
        {
            throw exception;
        }

        if (!passwordValidator.Validate(options.Password, out exception))
        {
            throw exception;
        }

        var salt = Guid.NewGuid();
        var hash = hasher.ComputeHash($"{salt};{options.Password}");

        var newUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            HashMethod = hasher.HashMethod,
            Login = options.Login,
            Salt = salt.ToString(),
            PasswordHash = hash,
            Email = options.Email,
        };

        return context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var user = await c.Set<UserEntity>()
                    .SingleOrDefaultAsync(x => x.Login == options.Login, cancellationToken);

                if (user is not null)
                {
                    throw new UserWithLoginExistsException(options.Login);
                }

                user = await c.Set<UserEntity>()
                    .SingleOrDefaultAsync(x => x.Email == options.Email, cancellationToken);

                if (user is not null)
                {
                    throw new UserWithEmilExistsException(options.Email);
                }

                await c.Set<UserEntity>().AddAsync(newUser, cancellationToken);
            }
        );
    }

    public async Task<TokenResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
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

                return tokenResult;
            }
            case Role.Service:
            {
                var tokenResult = tokenFactory.Create();

                return tokenResult;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public async Task UpdateVerificationCodeByLoginAsync(string login, CancellationToken cancellationToken)
    {
        var userEntity = await context.Set<UserEntity>()
            .SingleAsync(x => x.Login == login, cancellationToken);

        var verificationCode = randomString.GetRandom().ThrowIfNull();
        var hash = hasher.ComputeHash(verificationCode);
        userEntity.VerificationCodeMethod = hasher.HashMethod;
        userEntity.VerificationCodeHash = hash;
        userEntity.IsEmailVerified = false;
        await context.SaveChangesAsync(cancellationToken);

        await emailService.SendEmailAsync(
            "VerificationCode",
            userEntity.Email.ThrowIfNullOrWhiteSpace(),
            verificationCode,
            cancellationToken
        );
    }

    public async Task UpdateVerificationCodeByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var userEntity = await context.Set<UserEntity>()
            .SingleAsync(x => x.Email == email, cancellationToken);

        var verificationCode = randomString.GetRandom().ThrowIfNull();
        var hash = hasher.ComputeHash(verificationCode);
        userEntity.VerificationCodeMethod = hasher.HashMethod;
        userEntity.VerificationCodeHash = hash;
        userEntity.IsEmailVerified = false;
        await context.SaveChangesAsync(cancellationToken);
        await emailService.SendEmailAsync("VerificationCode", email, verificationCode, cancellationToken);
    }

    public async Task<bool> IsVerifiedByLoginAsync(string login, CancellationToken cancellationToken)
    {
        var userEntity = await context.Set<UserEntity>()
            .AsNoTracking()
            .SingleAsync(x => x.Login == login, cancellationToken);

        return userEntity.IsEmailVerified;
    }

    public async Task<bool> IsVerifiedByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var userEntity = await context.Set<UserEntity>()
            .AsNoTracking()
            .SingleAsync(x => x.Email == email, cancellationToken);

        return userEntity.IsEmailVerified;
    }

    public async Task VerifiedEmailByLoginAsync(
        string login,
        string verificationCode,
        CancellationToken cancellationToken
    )
    {
        var userEntity = await context.Set<UserEntity>()
            .SingleAsync(x => x.Login == login, cancellationToken);

        var h = hasherFactory.Create(userEntity.HashMethod.ThrowIfNullOrWhiteSpace());
        var hash = h.ComputeHash(verificationCode);

        if (hash != userEntity.VerificationCodeHash)
        {
            throw new Exception();
        }

        userEntity.IsEmailVerified = true;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task VerifiedEmailByEmailAsync(
        string email,
        string verificationCode,
        CancellationToken cancellationToken
    )
    {
        var userEntity = await context.Set<UserEntity>()
            .SingleAsync(x => x.Email == email, cancellationToken);

        var h = hasherFactory.Create(userEntity.HashMethod.ThrowIfNullOrWhiteSpace());
        var hash = h.ComputeHash(verificationCode);

        if (hash != userEntity.VerificationCodeHash)
        {
            throw new Exception();
        }

        userEntity.IsEmailVerified = true;
        await context.SaveChangesAsync(cancellationToken);
    }
}