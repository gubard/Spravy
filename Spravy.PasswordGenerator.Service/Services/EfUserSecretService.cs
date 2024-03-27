using Microsoft.EntityFrameworkCore;
using Spravy.Db.Extensions;
using Spravy.Domain.Extensions;
using Spravy.PasswordGenerator.Db.Contexts;
using Spravy.PasswordGenerator.Db.Models;
using Spravy.PasswordGenerator.Domain.Interfaces;
using Spravy.Service.Extensions;

namespace Spravy.PasswordGenerator.Service.Services;

public class EfUserSecretService : IUserSecretService
{
    private readonly UserSecretDbContext context;
    private readonly IHttpContextAccessor httpContextAccessor;

    public EfUserSecretService(UserSecretDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        this.context = context;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<byte[]> GetUserSecretAsync(CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.GetUserId().ToGuid();
        var user = await context.Set<UserSecretEntity>()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (user is not null)
        {
            return user.Secret;
        }

        var secret = Guid.NewGuid().ToByteArray();

        await context.ExecuteSaveChangesTransactionAsync(
            c => c.AddAsync(
                new UserSecretEntity
                {
                    Secret = secret,
                    UserId = userId,
                    Id = Guid.NewGuid(),
                },
                cancellationToken
            )
        );

        return secret;
    }
}