using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Spravy.Db.Extensions;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
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

    public async Task AddPasswordItemAsync(AddPasswordOptions options, CancellationToken cancellationToken)
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
    }

    public async Task<IEnumerable<PasswordItem>> GetPasswordItemsAsync(CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();
        var items = await context.Set<PasswordItemEntity>().AsNoTracking().ToArrayAsync(cancellationToken);

        return mapper.Map<IEnumerable<PasswordItem>>(items);
    }

    public async Task<PasswordItem> GetPasswordItemAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();
        var item = await context.FindAsync<PasswordItemEntity>(id);
        item = item.ThrowIfNull();

        return mapper.Map<PasswordItem>(item);
    }

    public async Task DeletePasswordItemAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();
        var item = await context.FindAsync<PasswordItemEntity>(id);
        item = item.ThrowIfNull();
        await context.ExecuteSaveChangesTransactionAsync(c => c.Remove(item));
    }

    public async Task<string> GeneratePasswordAsync(Guid id, CancellationToken cancellationToken)
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

    public async Task UpdatePasswordItemNameAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();
        var item = await context.FindAsync<PasswordItemEntity>(id);
        item = item.ThrowIfNull();
        await context.ExecuteSaveChangesTransactionAsync(_ => item.Name = name);
    }

    public async Task UpdatePasswordItemKeyAsync(Guid id, string key, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();
        var item = await context.FindAsync<PasswordItemEntity>(id);
        item = item.ThrowIfNull();
        await context.ExecuteSaveChangesTransactionAsync(_ => item.Key = key);
    }

    public async Task UpdatePasswordItemLengthAsync(Guid id, ushort length, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();
        var item = await context.FindAsync<PasswordItemEntity>(id);
        item = item.ThrowIfNull();
        await context.ExecuteSaveChangesTransactionAsync(_ => item.Length = length);
    }

    public async Task UpdatePasswordItemRegexAsync(Guid id, string regex, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();
        var item = await context.FindAsync<PasswordItemEntity>(id);
        item = item.ThrowIfNull();
        await context.ExecuteSaveChangesTransactionAsync(_ => item.Regex = regex);
    }

    public async Task UpdatePasswordItemIsAvailableNumberAsync(
        Guid id,
        bool isAvailableNumber,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();
        var item = await context.FindAsync<PasswordItemEntity>(id);
        item = item.ThrowIfNull();
        await context.ExecuteSaveChangesTransactionAsync(_ => item.IsAvailableNumber = isAvailableNumber);
    }

    public async Task UpdatePasswordItemIsAvailableLowerLatinAsync(
        Guid id,
        bool isAvailableLowerLatin,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();
        var item = await context.FindAsync<PasswordItemEntity>(id);
        item = item.ThrowIfNull();
        await context.ExecuteSaveChangesTransactionAsync(_ => item.IsAvailableLowerLatin = isAvailableLowerLatin);
    }

    public async Task UpdatePasswordItemIsAvailableSpecialSymbolsAsync(
        Guid id,
        bool isAvailableSpecialSymbols,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();
        var item = await context.FindAsync<PasswordItemEntity>(id);
        item = item.ThrowIfNull();
        await context.ExecuteSaveChangesTransactionAsync(
            _ => item.IsAvailableSpecialSymbols = isAvailableSpecialSymbols
        );
    }

    public async Task UpdatePasswordItemCustomAvailableCharactersAsync(
        Guid id,
        string customAvailableCharacters,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();
        var item = await context.FindAsync<PasswordItemEntity>(id);
        item = item.ThrowIfNull();
        await context.ExecuteSaveChangesTransactionAsync(
            _ => item.CustomAvailableCharacters = customAvailableCharacters
        );
    }

    public async Task UpdatePasswordItemIsAvailableUpperLatinAsync(
        Guid id,
        bool isAvailableUpperLatin,
        CancellationToken cancellationToken
    )
    {
        await using var context = dbContextFactory.Create();
        var item = await context.FindAsync<PasswordItemEntity>(id);
        item = item.ThrowIfNull();
        await context.ExecuteSaveChangesTransactionAsync(_ => item.IsAvailableUpperLatin = isAvailableUpperLatin);
    }
}