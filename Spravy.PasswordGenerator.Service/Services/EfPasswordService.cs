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
    private readonly IPasswordGenerator passwordGenerator;
    private readonly IUserSecretService userSecretService;

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
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(context => context.AtomicExecuteAsync(() =>
            {
                var item = mapper.Map<PasswordItemEntity>(options);
                item.Id = Guid.NewGuid();

                return context.AddEntityAsync(item, cancellationToken).ToResultOnlyAsync();
            }, cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<PasswordItem>>> GetPasswordItemsAsync(
        CancellationToken cancellationToken
    )
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(
                context => context.Set<PasswordItemEntity>()
                   .AsNoTracking()
                   .ToArrayEntitiesAsync(cancellationToken)
                   .IfSuccessAsync(items => mapper.Map<PasswordItem[]>(items.ToArray()).ToReadOnlyMemory().ToResult(),
                        cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<PasswordItem>> GetPasswordItemAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(
                context => context.FindEntityAsync<PasswordItemEntity>(id)
                   .IfSuccessAsync(item => mapper.Map<PasswordItem>(item).ToResult(), cancellationToken),
                cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> DeletePasswordItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(
                context => context.AtomicExecuteAsync(
                    () => context.FindEntityAsync<PasswordItemEntity>(id)
                       .IfSuccessAsync(context.RemoveEntity, cancellationToken), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<string>> GeneratePasswordAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(
                context => context.FindEntityAsync<PasswordItemEntity>(id)
                   .IfSuccessAsync(
                        item => userSecretService.GetUserSecretAsync(cancellationToken)
                           .IfSuccessAsync(
                                userSecret => passwordGenerator.GeneratePassword($"{userSecret}{item.Key}",
                                    mapper.Map<GeneratePasswordOptions>(item)), cancellationToken), cancellationToken),
                cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemNameAsync(
        Guid id,
        string name,
        CancellationToken cancellationToken
    )
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(context => context.AtomicExecuteAsync(() =>
                context.FindEntityAsync<PasswordItemEntity>(id)
                   .IfSuccessAsync(item =>
                    {
                        item.Name = name;

                        return Result.Success;
                    }, cancellationToken), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemKeyAsync(
        Guid id,
        string key,
        CancellationToken cancellationToken
    )
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(context => context.AtomicExecuteAsync(() =>
                context.FindEntityAsync<PasswordItemEntity>(id)
                   .IfSuccessAsync(item =>
                    {
                        item.Key = key;

                        return Result.Success;
                    }, cancellationToken), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemLengthAsync(
        Guid id,
        ushort length,
        CancellationToken cancellationToken
    )
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(context => context.AtomicExecuteAsync(() =>
                context.FindEntityAsync<PasswordItemEntity>(id)
                   .IfSuccessAsync(item =>
                    {
                        item.Length = length;

                        return Result.Success;
                    }, cancellationToken), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemRegexAsync(
        Guid id,
        string regex,
        CancellationToken cancellationToken
    )
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(context => context.AtomicExecuteAsync(() =>
                context.FindEntityAsync<PasswordItemEntity>(id)
                   .IfSuccessAsync(item =>
                    {
                        item.Regex = regex;

                        return Result.Success;
                    }, cancellationToken), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemIsAvailableNumberAsync(
        Guid id,
        bool isAvailableNumber,
        CancellationToken cancellationToken
    )
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(context => context.AtomicExecuteAsync(() =>
                context.FindEntityAsync<PasswordItemEntity>(id)
                   .IfSuccessAsync(item =>
                    {
                        item.IsAvailableNumber = isAvailableNumber;

                        return Result.Success;
                    }, cancellationToken), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemIsAvailableLowerLatinAsync(
        Guid id,
        bool isAvailableLowerLatin,
        CancellationToken cancellationToken
    )
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(context => context.FindEntityAsync<PasswordItemEntity>(id)
               .IfSuccessAsync(item =>
                {
                    item.IsAvailableLowerLatin = isAvailableLowerLatin;

                    return Result.Success;
                }, cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemIsAvailableSpecialSymbolsAsync(
        Guid id,
        bool isAvailableSpecialSymbols,
        CancellationToken cancellationToken
    )
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(context => context.AtomicExecuteAsync(() =>
                context.FindEntityAsync<PasswordItemEntity>(id)
                   .IfSuccessAsync(item =>
                    {
                        item.IsAvailableSpecialSymbols = isAvailableSpecialSymbols;

                        return Result.Success;
                    }, cancellationToken), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemCustomAvailableCharactersAsync(
        Guid id,
        string customAvailableCharacters,
        CancellationToken cancellationToken
    )
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(context => context.AtomicExecuteAsync(() =>
                context.FindEntityAsync<PasswordItemEntity>(id)
                   .IfSuccessAsync(item =>
                    {
                        item.CustomAvailableCharacters = customAvailableCharacters;

                        return Result.Success;
                    }, cancellationToken), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemIsAvailableUpperLatinAsync(
        Guid id,
        bool isAvailableUpperLatin,
        CancellationToken cancellationToken
    )
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(context => context.AtomicExecuteAsync(() =>
                context.FindEntityAsync<PasswordItemEntity>(id)
                   .IfSuccessAsync(item =>
                    {
                        item.IsAvailableUpperLatin = isAvailableUpperLatin;

                        return Result.Success;
                    }, cancellationToken), cancellationToken), cancellationToken);
    }
}