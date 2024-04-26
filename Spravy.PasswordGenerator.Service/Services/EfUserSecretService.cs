using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Spravy.Db.Extensions;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
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

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<byte>>> GetUserSecretAsync(
        CancellationToken cancellationToken
    )
    {
        var userId = httpContextAccessor.GetUserId().ToGuid();

        return context.AtomicExecuteAsync(
            () => context.Set<UserSecretEntity>()
                .AsNoTracking()
                .SingleOrDefaultEntityAsync(x => x.UserId == userId, cancellationToken)
                .IfSuccessAsync(user =>
                {
                    if (user is not null)
                    {
                        return user.Secret.ToReadOnlyMemory().ToResult().ToValueTaskResult().ConfigureAwait(false);
                    }

                    var secret = Guid.NewGuid().ToByteArray();

                    return context.Set<UserSecretEntity>().AddEntityAsync(
                        new UserSecretEntity
                        {
                            Secret = secret,
                            UserId = userId,
                            Id = Guid.NewGuid()
                        },
                        cancellationToken
                    ).IfSuccessAsync(_ => secret.ToReadOnlyMemory().ToResult(), cancellationToken);
                }, cancellationToken), cancellationToken);
    }
}