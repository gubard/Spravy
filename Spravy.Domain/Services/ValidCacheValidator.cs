namespace Spravy.Domain.Services;

public class ValidCacheValidator<TKey, TValue> : ICacheValidator<TKey, TValue>
{
    public bool IsValid(TKey key, TValue value)
    {
        return true;
    }
}