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

    public Cvtar AddPasswordItemAsync(AddPasswordOptions options, CancellationToken ct)
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(
                context => context.AtomicExecuteAsync(
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

    public Cvtar EditPasswordItemsAsync(EditPasswordItems options, CancellationToken ct)
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(
                context => context.AtomicExecuteAsync(
                    () =>
                    {
                        var ids = options.Ids.ToArray();

                        return context.Set<PasswordItemEntity>()
                           .Where(x => ids.Contains(x.Id))
                           .ToArrayEntitiesAsync(ct)
                           .IfSuccessForEachAsync(
                                item =>
                                {
                                    if (options.Name.IsEdit)
                                    {
                                        item.Name = options.Name.Value;
                                    }

                                    if (options.Key.IsEdit)
                                    {
                                        item.Key = options.Key.Value;
                                    }

                                    if (options.IsAvailableSpecialSymbols.IsEdit)
                                    {
                                        item.IsAvailableSpecialSymbols = options.IsAvailableSpecialSymbols.Value;
                                    }

                                    if (options.Length.IsEdit)
                                    {
                                        item.Length = options.Length.Value;
                                    }

                                    if (options.Login.IsEdit)
                                    {
                                        item.Login = options.Login.Value;
                                    }

                                    if (options.Regex.IsEdit)
                                    {
                                        item.Regex = options.Regex.Value;
                                    }

                                    if (options.CustomAvailableCharacters.IsEdit)
                                    {
                                        item.CustomAvailableCharacters = options.CustomAvailableCharacters.Value;
                                    }

                                    if (options.IsAvailableNumber.IsEdit)
                                    {
                                        item.IsAvailableNumber = options.IsAvailableNumber.Value;
                                    }

                                    if (options.IsAvailableLowerLatin.IsEdit)
                                    {
                                        item.IsAvailableLowerLatin = options.IsAvailableLowerLatin.Value;
                                    }

                                    if (options.IsAvailableUpperLatin.IsEdit)
                                    {
                                        item.IsAvailableUpperLatin = options.IsAvailableUpperLatin.Value;
                                    }

                                    return Result.AwaitableSuccess;
                                },
                                ct
                            );
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
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(
                context => context.Set<PasswordItemEntity>()
                   .AsNoTracking()
                   .ToArrayEntitiesAsync(ct)
                   .IfSuccessAsync(items => items.ToPasswordItem().ToResult(), ct),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<PasswordItem>> GetPasswordItemAsync(Guid id, CancellationToken ct)
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(
                context => context.GetEntityAsync<PasswordItemEntity>(id)
                   .IfSuccessAsync(item => item.ToPasswordItem().ToResult(), ct),
                ct
            );
    }

    public Cvtar DeletePasswordItemAsync(Guid id, CancellationToken ct)
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(
                context => context.AtomicExecuteAsync(
                    () => context.GetEntityAsync<PasswordItemEntity>(id).IfSuccessAsync(context.RemoveEntity, ct),
                    ct
                ),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result<string>> GeneratePasswordAsync(Guid id, CancellationToken ct)
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(
                context => context.GetEntityAsync<PasswordItemEntity>(id)
                   .IfSuccessAsync(
                        item => userSecretService.GetUserSecretAsync(ct)
                           .IfSuccessAsync(
                                userSecret => passwordGenerator.GeneratePassword(
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
}