namespace Spravy.PasswordGenerator.Service.Services;

public class EfUserSecretService : IUserSecretService
{
    private readonly UserSecretDbContext context;
    private readonly IHttpContextAccessor httpContextAccessor;

    public EfUserSecretService(
        UserSecretDbContext context,
        IHttpContextAccessor httpContextAccessor
    )
    {
        this.context = context;
        this.httpContextAccessor = httpContextAccessor;
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<byte>>> GetUserSecretAsync(
        CancellationToken ct
    )
    {
        var userId = httpContextAccessor.GetUserId().ToGuid();

        return context.AtomicExecuteAsync(
            () =>
                context
                    .Set<UserSecretEntity>()
                    .AsNoTracking()
                    .GetSecretByUserIdAsync(userId, ct)
                    .IfSuccessAsync(
                        user =>
                            user.Secret.ToReadOnlyMemory()
                                .ToResult()
                                .ToValueTaskResult()
                                .ConfigureAwait(false),
                        errors =>
                        {
                            if (errors.Length != 1)
                            {
                                return new Result<ReadOnlyMemory<byte>>(errors)
                                    .ToValueTaskResult()
                                    .ConfigureAwait(false);
                            }

                            if (errors.Span[0].Id != NotFoundUserSecretError.MainId)
                            {
                                return new Result<ReadOnlyMemory<byte>>(errors.Span[0])
                                    .ToValueTaskResult()
                                    .ConfigureAwait(false);
                            }

                            var secret = Guid.NewGuid().ToByteArray();

                            return context
                                .Set<UserSecretEntity>()
                                .AddEntityAsync(
                                    new()
                                    {
                                        Secret = secret,
                                        UserId = userId,
                                        Id = Guid.NewGuid(),
                                    },
                                    ct
                                )
                                .IfSuccessAsync(_ => secret.ToReadOnlyMemory().ToResult(), ct);
                        },
                        ct
                    ),
            ct
        );
    }
}
