using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Spravy.Authentication.Db.Contexts;
using Spravy.Authentication.Db.Models;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Service.Interfaces;
using Spravy.Db.Extensions;
using Spravy.Domain.Enums;
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

    public EfAuthenticationService(
        SpravyDbAuthenticationDbContext context,
        IHasher hasher,
        IFactory<string, IHasher> hasherFactory,
        ITokenFactory tokenFactory,
        IMapper mapper
    )
    {
        this.context = context;
        this.hasher = hasher;
        this.hasherFactory = hasherFactory;
        this.tokenFactory = tokenFactory;
        this.mapper = mapper;
    }

    public async Task<TokenResult> LoginAsync(User user, CancellationToken cancellationToken)
    {
        var userEntity = await context.Set<UserEntity>().AsNoTracking().SingleAsync(x => x.Login == user.Login,  cancellationToken);
        var newHasher = hasherFactory.Create(userEntity.HashMethod.ThrowIfNullOrWhiteSpace());
        var hash = newHasher.ComputeHash($"{userEntity.Salt};{user.Password}");

        if (hash != userEntity.PasswordHash)
        {
            throw new Exception("Wrong password.");
        }

        var userTokenClaims = mapper.Map<UserTokenClaims>(userEntity);
        var tokenResult = tokenFactory.Create(userTokenClaims);

        return tokenResult;
    }

    public async Task CreateUserAsync(CreateUserOptions options, CancellationToken cancellationToken)
    {
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

        await context.ExecuteSaveChangesTransactionAsync(c => c.Set<UserEntity>().AddAsync(newUser, cancellationToken));
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
                    .SingleAsync(x => x.Login == loginClaim.Value,  cancellationToken);

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