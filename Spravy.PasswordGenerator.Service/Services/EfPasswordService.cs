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

    public EfPasswordService(IFactory<PasswordDbContext> dbContextFactory, IMapper mapper)
    {
        this.dbContextFactory = dbContextFactory;
        this.mapper = mapper;
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

    public async Task RemovePasswordItemAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var context = dbContextFactory.Create();
        var item = await context.FindAsync<PasswordItemEntity>(id);
        item = item.ThrowIfNull();
        await context.ExecuteSaveChangesTransactionAsync(c => c.Remove(item));
    }
}