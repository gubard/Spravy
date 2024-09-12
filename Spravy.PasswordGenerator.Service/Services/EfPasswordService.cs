namespace Spravy.PasswordGenerator.Service.Services;

public class EfPasswordService : IPasswordService
{
    private readonly IFactory<PasswordDbContext> dbContextFactory;
    private readonly IPasswordGenerator passwordGenerator;
    private readonly IUserSecretService userSecretService;

    public EfPasswordService(
        IFactory<PasswordDbContext> dbContextFactory,
        IUserSecretService userSecretService,
        IPasswordGenerator passwordGenerator
    )
    {
        this.dbContextFactory = dbContextFactory;
        this.userSecretService = userSecretService;
        this.passwordGenerator = passwordGenerator;
    }

    public ConfiguredValueTaskAwaitable<Result> AddPasswordItemAsync(
        AddPasswordOptions options,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                        {
                            var item = options.ToPasswordItemEntity();
                            item.Id = Guid.NewGuid();

                            return context.AddEntityAsync(item, ct).ToResultOnlyAsync();
                        },
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<PasswordItem>>> GetPasswordItemsAsync(
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context
                        .Set<PasswordItemEntity>()
                        .AsNoTracking()
                        .ToArrayEntitiesAsync(ct)
                        .IfSuccessAsync(items => items.ToPasswordItem().ToResult(), ct),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<PasswordItem>> GetPasswordItemAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context
                        .GetEntityAsync<PasswordItemEntity>(id)
                        .IfSuccessAsync(item => item.ToPasswordItem().ToResult(), ct),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> DeletePasswordItemAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<PasswordItemEntity>(id)
                                .IfSuccessAsync(context.RemoveEntity, ct),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<string>> GeneratePasswordAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context
                        .GetEntityAsync<PasswordItemEntity>(id)
                        .IfSuccessAsync(
                            item =>
                                userSecretService
                                    .GetUserSecretAsync(ct)
                                    .IfSuccessAsync(
                                        userSecret =>
                                            passwordGenerator.GeneratePassword(
                                                $"{userSecret}{item.Key}",
                                                item.ToGeneratePasswordOptions()
                                            ),
                                        ct
                                    ),
                            ct
                        ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemNameAsync(
        Guid id,
        string name,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<PasswordItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.Name = name;

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemKeyAsync(
        Guid id,
        string key,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<PasswordItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.Key = key;

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemLengthAsync(
        Guid id,
        ushort length,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<PasswordItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.Length = length;

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemRegexAsync(
        Guid id,
        string regex,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<PasswordItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.Regex = regex;

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemIsAvailableNumberAsync(
        Guid id,
        bool isAvailableNumber,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<PasswordItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.IsAvailableNumber = isAvailableNumber;

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemIsAvailableLowerLatinAsync(
        Guid id,
        bool isAvailableLowerLatin,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context
                        .GetEntityAsync<PasswordItemEntity>(id)
                        .IfSuccessAsync(
                            item =>
                            {
                                item.IsAvailableLowerLatin = isAvailableLowerLatin;

                                return Result.Success;
                            },
                            ct
                        ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemIsAvailableSpecialSymbolsAsync(
        Guid id,
        bool isAvailableSpecialSymbols,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<PasswordItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.IsAvailableSpecialSymbols = isAvailableSpecialSymbols;

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemCustomAvailableCharactersAsync(
        Guid id,
        string customAvailableCharacters,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<PasswordItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.CustomAvailableCharacters = customAvailableCharacters;

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemIsAvailableUpperLatinAsync(
        Guid id,
        bool isAvailableUpperLatin,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<PasswordItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.IsAvailableUpperLatin = isAvailableUpperLatin;

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemLoginAsync(
        Guid id,
        string login,
        CancellationToken ct
    )
    {
        return dbContextFactory
            .Create()
            .IfSuccessDisposeAsync(
                context =>
                    context.AtomicExecuteAsync(
                        () =>
                            context
                                .GetEntityAsync<PasswordItemEntity>(id)
                                .IfSuccessAsync(
                                    item =>
                                    {
                                        item.Login = login;

                                        return Result.Success;
                                    },
                                    ct
                                ),
                        ct
                    ),
                ct
            );
    }
}
