namespace Spravy.Ui.Features.PasswordGenerator.Services;

public class PasswordItemCache : IPasswordItemCache
{
    private readonly Dictionary<Guid, PasswordItemEntityNotify> cache;

    public PasswordItemCache()
    {
        cache = new();
    }

    public Result<PasswordItemEntityNotify> GetPasswordItem(Guid id)
    {
        if (cache.TryGetValue(id, out var value))
        {
            return value.ToResult();
        }

        var result = new PasswordItemEntityNotify { Id = id, Name = "Loading...", };

        if (cache.TryAdd(id, result))
        {
            return result.ToResult();
        }

        return cache[id].ToResult();
    }

    public Result UpdateUi(PasswordItem passwordItem)
    {
        return GetPasswordItem(passwordItem.Id)
            .IfSuccess(item =>
            {
                item.Name = passwordItem.Name;

                return Result.Success;
            });
    }
}
