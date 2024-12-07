using System.Collections.Frozen;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Spravy.Db.Models;

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

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetChildrenPasswordItemIdsAsync(
        OptionStruct<Guid> id,
        CancellationToken ct
    )
    {
        var parentId = id.GetValueOrNull();

        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(
                context => context.Set<PasswordItemEntity>()
                   .AsNoTracking()
                   .Where(x => x.ParentId == parentId)
                   .OrderBy(x => x.OrderIndex)
                   .Select(x => x.Id)
                   .ToArrayEntitiesAsync(ct),
                ct
            );
    }

    public ConfiguredCancelableAsyncEnumerable<Result<ReadOnlyMemory<PasswordItem>>> GetPasswordItemsAsync(
        ReadOnlyMemory<Guid> ids,
        uint chunkSize,
        CancellationToken ct
    )
    {
        return GetPasswordItemsCore(ids, chunkSize, ct).ConfigureAwait(false);
    }

    private async IAsyncEnumerable<Result<ReadOnlyMemory<PasswordItem>>> GetPasswordItemsCore(
        ReadOnlyMemory<Guid> ids,
        uint chunkSize,
        [EnumeratorCancellation] CancellationToken ct
    )
    {
        if (ids.IsEmpty)
        {
            yield break;
        }

        for (uint i = 0; i < ids.Length; i += chunkSize)
        {
            var size = i + chunkSize > ids.Length ? (int)(ids.Length - i) : (int)chunkSize;
            var range = ids.Slice((int)i, size);

            if (range.IsEmpty)
            {
                yield break;
            }

            var items = await GetPasswordItemsAsync(range, ct);

            if (items.IsHasError)
            {
                yield return items;

                yield break;
            }

            yield return items;
        }
    }

    private ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<PasswordItem>>> GetPasswordItemsAsync(
        ReadOnlyMemory<Guid> ids,
        CancellationToken ct
    )
    {
        return dbContextFactory.Create()
           .IfSuccessDisposeAsync(
                context => GetAllChildrenAsync(context, ids, false, ct)
                   .IfSuccessAsync(
                        items => items.Values
                           .Where(x => ids.Contains(x.Id))
                           .OrderBy(x => x.OrderIndex)
                           .ToArray()
                           .ToReadOnlyMemory()
                           .IfSuccessForEach(item => item.ToPasswordItem().ToResult()),
                        ct
                    ),
                ct
            );
    }

    private ConfiguredValueTaskAwaitable<Result<FrozenDictionary<Guid, PasswordItemEntity>>> GetAllChildrenAsync(
        PasswordDbContext context,
        ReadOnlyMemory<Guid> ids,
        bool tracking,
        CancellationToken ct
    )
    {
        var parameters = CreateSqlRawParametersForAllChildren(ids);
        var query = context.Set<PasswordItemEntity>().FromSqlRaw(parameters.Sql, parameters.Parameters.ToArray());

        if (tracking)
        {
            query.AsTracking();
        }
        else
        {
            query.AsNoTracking();
        }

        return query.ToArrayEntitiesAsync(ct)
           .IfSuccessAsync(
                items =>
                {
                    var dictionary = new Dictionary<Guid, PasswordItemEntity>();

                    foreach (var item in items.Span)
                    {
                        dictionary.TryAdd(item.Id, item);
                    }

                    return dictionary.ToFrozenDictionary().ToResult();
                },
                ct
            );
    }

    private SqlRawParameters CreateSqlRawParametersForAllChildren(ReadOnlyMemory<Guid> ids)
    {
        var idsString = Enumerable.Range(0, ids.Length).Select(i => $"@Id{i}").JoinString(", ");
        var parameters = new DbParameter[ids.Length];

        for (var i = 0; i < ids.Length; i++)
        {
            parameters[i] = new SqliteParameter($"@Id{i}", ids.Span[i]);
        }

        return new(
            $"""
            WITH RECURSIVE hierarchy(
                     Id,
                     Name,
                     Login,
                     Key,
                     IsAvailableUpperLatin,
                     IsAvailableLowerLatin,
                     IsAvailableNumber,
                     IsAvailableSpecialSymbols,
                     CustomAvailableCharacters,
                     Length,
                     Regex,
                     Type,
                     ParentId,
                     OrderIndex
                 ) AS (
                     SELECT
                     Id,
                     Name,
                     Login,
                     Key,
                     IsAvailableUpperLatin,
                     IsAvailableLowerLatin,
                     IsAvailableNumber,
                     IsAvailableSpecialSymbols,
                     CustomAvailableCharacters,
                     Length,
                     Regex,
                     Type,
                     ParentId,
                     OrderIndex
                     FROM PasswordItems
                     WHERE Id IN ({idsString})
            
                     UNION ALL
            
                     SELECT
                     t.Id,
                     t.Name,
                     t.Login,
                     t.Key,
                     t.IsAvailableUpperLatin,
                     t.IsAvailableLowerLatin,
                     t.IsAvailableNumber,
                     t.IsAvailableSpecialSymbols,
                     t.CustomAvailableCharacters,
                     t.Length,
                     t.Regex,
                     t.Type,
                     t.ParentId,
                     t.OrderIndex
                     FROM PasswordItems t
                     INNER JOIN hierarchy h ON t.ParentId = h.Id
                 )
                 SELECT * FROM hierarchy;
            """,
            parameters
        );
    }
}