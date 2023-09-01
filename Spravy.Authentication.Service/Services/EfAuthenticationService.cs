using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Spravy.Authentication.Db.Contexts;
using Spravy.Authentication.Db.Models;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Service.Interfaces;
using Spravy.Domain.Interfaces;

namespace Spravy.Authentication.Service.Services;

public class EfAuthenticationService : IAuthenticationService
{
    private readonly SpravyAuthenticationDbContext context;
    private readonly IHasher hasher;
    private readonly IFactory<string, IHasher> hasherFactory;
    private readonly ITokenFactory tokenFactory;
    private readonly IMapper mapper;

    public EfAuthenticationService(
        SpravyAuthenticationDbContext context,
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

    public async Task<TokenResult> LoginAsync(User user)
    {
        var userEntity = await context.Set<UserEntity>().AsNoTracking().SingleAsync(x => x.Login == user.Login);
        var newHasher = hasherFactory.Create(userEntity.HashMethod);
        var hash = newHasher.ComputeHash($"{userEntity.Salt};{user.Password}");

        if (hash != userEntity.PasswordHash)
        {
            throw new Exception("Wrong password.");
        }

        var userTokenClaims = mapper.Map<UserTokenClaims>(userEntity);
        var tokenResult = tokenFactory.Create(userTokenClaims);

        return tokenResult;
    }

    public async Task<TokenResult> CreateUserAsync(CreateUserOptions options)
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
        };

        var entityEntry = await context.Set<UserEntity>().AddAsync(newUser);
        await context.SaveChangesAsync();
        var userTokenClaims = mapper.Map<UserTokenClaims>(entityEntry.Entity);
        var tokenResult = tokenFactory.Create(userTokenClaims);

        return tokenResult;
    }
}