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

        var result = new PasswordItemEntityNotify
        {
            Id = id,
            Name = "Loading...",
        };

        if (cache.TryAdd(id, result))
        {
            return result.ToResult();
        }

        return cache[id].ToResult();
    }

    public Result UpdateUi(PasswordItem passwordItem)
    {
        return GetPasswordItem(passwordItem.Id)
           .IfSuccess(
                item =>
                {
                    item.Name = passwordItem.Name;
                    item.IsAvailableNumber = passwordItem.IsAvailableNumber;
                    item.IsAvailableUpperLatin = passwordItem.IsAvailableUpperLatin;
                    item.Regex = passwordItem.Regex;
                    item.Key = passwordItem.Key;
                    item.Length = passwordItem.Length;
                    item.IsAvailableLowerLatin = passwordItem.IsAvailableLowerLatin;
                    item.CustomAvailableCharacters = passwordItem.CustomAvailableCharacters;
                    item.IsAvailableSpecialSymbols = passwordItem.IsAvailableSpecialSymbols;
                    item.Login = passwordItem.Login;

                    return Result.Success;
                }
            );
    }
}