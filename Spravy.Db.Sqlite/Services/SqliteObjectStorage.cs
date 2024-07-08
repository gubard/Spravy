using Spravy.Db.Extensions;

namespace Spravy.Db.Sqlite.Services;

public class SqliteObjectStorage : IObjectStorage
{
    private readonly ISerializer serializer;
    private readonly StorageDbContext storageDbContext;

    public SqliteObjectStorage(StorageDbContext storageDbContext, ISerializer serializer)
    {
        this.storageDbContext = storageDbContext;
        this.serializer = serializer;
    }

    public ConfiguredValueTaskAwaitable<Result<bool>> IsExistsAsync(string id, CancellationToken ct)
    {
        return IsExistsCore(id, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<Result> DeleteAsync(string id, CancellationToken ct)
    {
        return DeleteCore(id, ct).ConfigureAwait(false);
    }

    private async ValueTask<Result> DeleteCore(string id, CancellationToken ct)
    {
        var item = await storageDbContext.FindAsync<StorageEntity>(id);
        item = item.ThrowIfNull();
        storageDbContext.Set<StorageEntity>().Remove(item);
        await storageDbContext.SaveChangesAsync(ct);

        return Result.Success;
    }

    private async ValueTask<Result<bool>> IsExistsCore(string id, CancellationToken ct)
    {
        var item = await storageDbContext.FindAsync<StorageEntity>(id);

        return (item is not null).ToResult();
    }

    public ConfiguredValueTaskAwaitable<Result<TObject>> GetObjectAsync<TObject>(
        string id,
        CancellationToken ct
    )
        where TObject : notnull
    {
        return GetObjectCore<TObject>(id, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<Result> SaveObjectAsync(
        string id,
        object obj,
        CancellationToken ct
    )
    {
        return storageDbContext.AtomicExecuteAsync(
            () =>
                storageDbContext
                    .FindEntityAsync<StorageEntity>(id)
                    .IfSuccessAsync(
                        item =>
                            new MemoryStream()
                                .ToResult()
                                .IfSuccessDisposeAsync(
                                    stream =>
                                        serializer
                                            .SerializeAsync(obj, stream, ct)
                                            .IfSuccessAsync(
                                                () =>
                                                {
                                                    stream.Position = 0;
                                                    var value = stream.ToArray();

                                                    if (item.TryGetValue(out _))
                                                    {
                                                        return storageDbContext
                                                            .Set<StorageEntity>()
                                                            .Where(x => x.Id == id)
                                                            .ExecuteUpdateEntityAsync(
                                                                x =>
                                                                    x.SetProperty(
                                                                        p => p.Value,
                                                                        value
                                                                    ),
                                                                ct
                                                            )
                                                            .ToResultOnlyAsync();
                                                    }

                                                    var entity = new StorageEntity
                                                    {
                                                        Id = id,
                                                        Value = value,
                                                    };

                                                    return storageDbContext
                                                        .Set<StorageEntity>()
                                                        .AddEntityAsync(entity, ct)
                                                        .ToResultOnlyAsync();
                                                },
                                                ct
                                            ),
                                    ct
                                ),
                        ct
                    ),
            ct
        );
    }

    private async ValueTask<Result<TObject>> GetObjectCore<TObject>(string id, CancellationToken ct)
        where TObject : notnull
    {
        var item = await storageDbContext.FindAsync<StorageEntity>(id);
        item = item.ThrowIfNull();
        await using var stream = item.Value.ToMemoryStream();
        stream.Position = 0;
        var result = await serializer.DeserializeAsync<TObject>(stream, ct);

        return result;
    }
}
