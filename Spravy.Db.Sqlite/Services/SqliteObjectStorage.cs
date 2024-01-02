using Spravy.Db.Models;
using Spravy.Db.Services;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;

namespace Spravy.Db.Sqlite.Services;

public class SqliteObjectStorage : IObjectStorage
{
    private readonly StorageDbContext storageDbContext;
    private readonly ISerializer serializer;

    public SqliteObjectStorage(StorageDbContext storageDbContext, ISerializer serializer)
    {
        this.storageDbContext = storageDbContext;
        this.serializer = serializer;
    }

    public async Task<bool> IsExistsAsync(string id)
    {
        var item = await storageDbContext.FindAsync<StorageEntity>(id);

        return item is not null;
    }

    public async Task DeleteAsync(string id)
    {
        var item = await storageDbContext.FindAsync<StorageEntity>(id);
        item = item.ThrowIfNull();
        storageDbContext.Set<StorageEntity>().Remove(item);
        await storageDbContext.SaveChangesAsync();
    }

    public async Task SaveObjectAsync(string id, object obj)
    {
        var item = await storageDbContext.FindAsync<StorageEntity>(id);
        await using var steam = new MemoryStream();
        await serializer.SerializeAsync(obj, steam);
        steam.Position = 0;

        if (item is null)
        {
            await storageDbContext.Set<StorageEntity>()
                .AddAsync(
                    new StorageEntity
                    {
                        Id = id,
                        Value = steam.ToArray()
                    }
                );
        }
        else
        {
            item.Value = steam.ToArray();
        }

        await storageDbContext.SaveChangesAsync();
    }

    public async Task<TObject> GetObjectAsync<TObject>(string id)
    {
        var item = await storageDbContext.FindAsync<StorageEntity>(id);
        item = item.ThrowIfNull();
        await using var stream = item.Value.ToMemoryStream();
        stream.Position = 0;
        var result = await serializer.DeserializeAsync<TObject>(stream);

        return result;
    }
}