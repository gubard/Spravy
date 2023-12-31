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

    public EfAuthenticationService(
        SpravyDbAuthenticationDbContext context,
        IHasher hasher,
        IFactory<string, IHasher> hasherFactory,
        ITokenFactory tokenFactory,
        IMapper mapper,
        IValidator<SpravyException, string> loginValidator,
        IValidator<SpravyException, string> passwordValidator
    )
    {
        this.context = context;
        this.hasher = hasher;
        this.hasherFactory = hasherFactory;
        this.tokenFactory = tokenFactory;
        this.mapper = mapper;
        this.loginValidator = loginValidator;
        this.passwordValidator = passwordValidator;
    }

    public async Task<TokenResult> LoginAsync(User user, CancellationToken cancellationToken)
    {
        var userEntity = await context.Set<UserEntity>()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Login == user.Login, cancellationToken);

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
                    .SingleAsync(x => x.Login == loginClaim.Value, cancellationToken);

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
}