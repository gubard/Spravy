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

    public ConfiguredValueTaskAwaitable<Result<bool>> IsExistsAsync(string id)
    {
        return IsExistsCore(id).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<Result> DeleteAsync(string id)
    {
        return DeleteCore(id).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<Result> SaveObjectAsync(string id, object obj)
    {
        return SaveObjectCore(id, obj).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<Result<TObject>> GetObjectAsync<TObject>(string id)
    {
        return GetObjectCore<TObject>(id).ConfigureAwait(false);
    }

    private async ValueTask<Result<bool>> IsExistsCore(string id)
    {
        var item = await storageDbContext.FindAsync<StorageEntity>(id);

        return (item is not null).ToResult();
    }

    private async ValueTask<Result> DeleteCore(string id)
    {
        var item = await storageDbContext.FindAsync<StorageEntity>(id);
        item = item.ThrowIfNull();
        storageDbContext.Set<StorageEntity>().Remove(item);
        await storageDbContext.SaveChangesAsync();

        return Result.Success;
    }

    private async ValueTask<Result> SaveObjectCore(string id, object obj)
    {
        var item = await storageDbContext.FindAsync<StorageEntity>(id);
        await using var steam = new MemoryStream();
        var result = await serializer.SerializeAsync(obj, steam);

        if (result.IsHasError)
        {
            return result;
        }

        steam.Position = 0;

        if (item is null)
        {
            var entity = new StorageEntity
            {
                Id = id,
                Value = steam.ToArray(),
            };

            await storageDbContext.Set<StorageEntity>().AddAsync(entity);
        }
        else
        {
            item.Value = steam.ToArray();
        }

        await storageDbContext.SaveChangesAsync();

        return Result.Success;
    }

    private async ValueTask<Result<TObject>> GetObjectCore<TObject>(string id)
    {
        var t = await storageDbContext.Set<StorageEntity>().ToArrayAsync();
        var item = await storageDbContext.FindAsync<StorageEntity>(id);
        item = item.ThrowIfNull();
        await using var stream = item.Value.ToMemoryStream();
        stream.Position = 0;
        var result = await serializer.DeserializeAsync<TObject>(stream);

        return result;
    }
}