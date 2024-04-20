using System.Runtime.CompilerServices;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Spravy.Db.Extensions;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.PasswordGenerator.Db.Contexts;
using Spravy.PasswordGenerator.Db.Models;
using Spravy.PasswordGenerator.Domain.Interfaces;
using Spravy.PasswordGenerator.Domain.Models;

namespace Spravy.PasswordGenerator.Service.Services;

public class EfPasswordService : IPasswordService
{
    private readonly IFactory<PasswordDbContext> dbContextFactory;
    private readonly IMapper mapper;
    private readonly IUserSecretService userSecretService;
    private readonly IPasswordGenerator passwordGenerator;

    public EfPasswordService(
        IFactory<PasswordDbContext> dbContextFactory,
        IMapper mapper,
        IUserSecretService userSecretService,
        IPasswordGenerator passwordGenerator
    )
    {
        this.dbContextFactory = dbContextFactory;
        this.mapper = mapper;
        this.userSecretService = userSecretService;
        this.passwordGenerator = passwordGenerator;
    }

    public ConfiguredValueTaskAwaitable<Result> AddPasswordItemAsync(
        AddPasswordOptions options,
        CancellationToken cancellationToken
    )
    {
        return AddPasswordItemCore(options, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> AddPasswordItemCore(AddPasswordOptions options, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(
            c =>
            {
                var item = mapper.Map<PasswordItemEntity>(options);
                item.Id = Guid.NewGuid();

                return c.AddAsync(item, cancellationToken);
            }
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<PasswordItem>>> GetPasswordItemsAsync(
        CancellationToken cancellationToken
    )
    {
        return GetPasswordItemsCore(cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result<ReadOnlyMemory<PasswordItem>>> GetPasswordItemsCore(
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();
        var items = await context.Set<PasswordItemEntity>().AsNoTracking().ToArrayAsync(cancellationToken);

        return mapper.Map<PasswordItem[]>(items).ToReadOnlyMemory().ToResult();
    }

    public ConfiguredValueTaskAwaitable<Result<PasswordItem>> GetPasswordItemAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return GetPasswordItemCore(id, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result<PasswordItem>> GetPasswordItemCore(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();
        var item = await context.FindAsync<PasswordItemEntity>(id);
        item = item.ThrowIfNull();

        return mapper.Map<PasswordItem>(item).ToResult();
    }

    public ConfiguredValueTaskAwaitable<Result> DeletePasswordItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return DeletePasswordItemCore(id, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> DeletePasswordItemCore(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();
        var item = await context.FindAsync<PasswordItemEntity>(id);
        item = item.ThrowIfNull();
        await context.ExecuteSaveChangesTransactionAsync(c => c.Remove(item), cancellationToken);

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result<string>> GeneratePasswordAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return GeneratePasswordCore(id, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result<string>> GeneratePasswordCore(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();
        var item = await context.FindAsync<PasswordItemEntity>(id);
        item = item.ThrowIfNull();
        var userSecret = await userSecretService.GetUserSecretAsync(cancellationToken);

        return passwordGenerator.GeneratePassword(
            $"{userSecret}{item.Key}",
            mapper.Map<GeneratePasswordOptions>(item)
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemNameAsync(
        Guid id,
        string name,
        CancellationToken cancellationToken
    )
    {
        return UpdatePasswordItemNameCore(id, name, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> UpdatePasswordItemNameCore(
        Guid id,
        string name,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(async c =>
        {
            var item = await c.FindAsync<PasswordItemEntity>(id);
            item = item.ThrowIfNull();
            item.Name = name;
        }, cancellationToken);

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemKeyAsync(
        Guid id,
        string key,
        CancellationToken cancellationToken
    )
    {
        return UpdatePasswordItemKeyCore(id, key, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> UpdatePasswordItemKeyCore(Guid id, string key, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(async c =>
        {
            var item = await c.FindAsync<PasswordItemEntity>(id);
            item = item.ThrowIfNull();
            item.Key = key;
        }, cancellationToken);

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemLengthAsync(
        Guid id,
        ushort length,
        CancellationToken cancellationToken
    )
    {
        return UpdatePasswordItemLengthCore(id, length, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> UpdatePasswordItemLengthCore(
        Guid id,
        ushort length,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(async c =>
        {
            var item = await c.FindAsync<PasswordItemEntity>(id);
            item = item.ThrowIfNull();
            item.Length = length;
        }, cancellationToken);

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemRegexAsync(
        Guid id,
        string regex,
        CancellationToken cancellationToken
    )
    {
        return UpdatePasswordItemRegexCore(id, regex, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> UpdatePasswordItemRegexCore(
        Guid id,
        string regex,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(async c =>
        {
            var item = await c.FindAsync<PasswordItemEntity>(id);
            item = item.ThrowIfNull();
            item.Regex = regex;
        }, cancellationToken);

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemIsAvailableNumberAsync(
        Guid id,
        bool isAvailableNumber,
        CancellationToken cancellationToken
    )
    {
        return UpdatePasswordItemIsAvailableNumberCore(id, isAvailableNumber, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<Result> UpdatePasswordItemIsAvailableNumberCore(
        Guid id,
        bool isAvailableNumber,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(async c =>
            {
                var item = await c.FindAsync<PasswordItemEntity>(id);
                item = item.ThrowIfNull();
                item.IsAvailableNumber = isAvailableNumber;
            },
            cancellationToken);

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemIsAvailableLowerLatinAsync(
        Guid id,
        bool isAvailableLowerLatin,
        CancellationToken cancellationToken
    )
    {
        return UpdatePasswordItemIsAvailableLowerLatinCore(id, isAvailableLowerLatin, cancellationToken)
            .ConfigureAwait(false);
    }

    private async ValueTask<Result> UpdatePasswordItemIsAvailableLowerLatinCore(
        Guid id,
        bool isAvailableLowerLatin,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(async c =>
            {
                var item = await c.FindAsync<PasswordItemEntity>(id);
                item = item.ThrowIfNull();
                item.IsAvailableLowerLatin = isAvailableLowerLatin;
            },
            cancellationToken);

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemIsAvailableSpecialSymbolsAsync(
        Guid id,
        bool isAvailableSpecialSymbols,
        CancellationToken cancellationToken
    )
    {
        return UpdatePasswordItemIsAvailableSpecialSymbolsCore(id, isAvailableSpecialSymbols, cancellationToken)
            .ConfigureAwait(false);
    }

    private async ValueTask<Result> UpdatePasswordItemIsAvailableSpecialSymbolsCore(
        Guid id,
        bool isAvailableSpecialSymbols,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();


        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.FindAsync<PasswordItemEntity>(id);
                item = item.ThrowIfNull();
                item.IsAvailableSpecialSymbols = isAvailableSpecialSymbols;
            }, cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemCustomAvailableCharactersAsync(
        Guid id,
        string customAvailableCharacters,
        CancellationToken cancellationToken
    )
    {
        return UpdatePasswordItemCustomAvailableCharactersCore(id, customAvailableCharacters, cancellationToken)
            .ConfigureAwait(false);
    }

    private async ValueTask<Result> UpdatePasswordItemCustomAvailableCharactersCore(
        Guid id,
        string customAvailableCharacters,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();


        await context.ExecuteSaveChangesTransactionAsync(
            async c =>
            {
                var item = await c.FindAsync<PasswordItemEntity>(id);
                item = item.ThrowIfNull();
                item.CustomAvailableCharacters = customAvailableCharacters;
            }, cancellationToken
        );

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemIsAvailableUpperLatinAsync(
        Guid id,
        bool isAvailableUpperLatin,
        CancellationToken cancellationToken
    )
    {
        return UpdatePasswordItemIsAvailableUpperLatinCore(id, isAvailableUpperLatin, cancellationToken)
            .ConfigureAwait(false);
    }

    private async ValueTask<Result> UpdatePasswordItemIsAvailableUpperLatinCore(
        Guid id,
        bool isAvailableUpperLatin,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();

        await context.ExecuteSaveChangesTransactionAsync(async c =>
            {
                var item = await c.FindAsync<PasswordItemEntity>(id);
                item = item.ThrowIfNull();
                item.IsAvailableUpperLatin = isAvailableUpperLatin;
            },
            cancellationToken);

        return Result.Success;
    }
}