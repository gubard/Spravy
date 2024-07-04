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
            () => SaveObjectCore(id, obj, ct).ConfigureAwait(false),
            ct
        );
    }

    private async ValueTask<Result> SaveObjectCore(string id, object obj, CancellationToken ct)
    {
        var item = await storageDbContext.FindAsync<StorageEntity>(id);
        await using var steam = new MemoryStream();
        var result = await serializer.SerializeAsync(obj, steam, ct);

        if (result.IsHasError)
        {
            return result;
        }

        steam.Position = 0;

        if (item is null)
        {
            var entity = new StorageEntity { Id = id, Value = steam.ToArray(), };
            await storageDbContext.Set<StorageEntity>().AddAsync(entity, ct);
        }
        else
        {
            item.Value = steam.ToArray();
        }

        return Result.Success;
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
