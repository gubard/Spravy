namespace Spravy.Domain.Services;

public class CacheFactory<TKey, TValue> : IFactory<TKey, TValue>, ICache<TKey, TValue>
    where TValue : notnull where TKey : notnull
{
    private static readonly Dictionary<TKey, TValue> cache = new();
    private readonly ICacheValidator<TKey, TValue> cacheValidator;

    private readonly IFactory<TKey, TValue> factory;

    public CacheFactory(IFactory<TKey, TValue> factory, ICacheValidator<TKey, TValue> cacheValidator)
    {
        this.factory = factory;
        this.cacheValidator = cacheValidator;
    }

    public bool TryGetCacheValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return cache.TryGetValue(key, out value);
    }

    public Result<TValue> Create(TKey key)
    {
        if (!cache.TryGetValue(key, out var value))
        {
            return CreateNewValue(key);
        }

        if (cacheValidator.IsValid(key, value))
        {
            return value.ToResult();
        }

        cache.Remove(key);

        return CreateNewValue(key);
    }

    private Result<TValue> CreateNewValue(TKey key)
    {
        return factory.Create(key)
           .IfSuccess(
                value =>
                {
                    cache.Add(key, value);

                    return value.ToResult();
                }
            );
    }
}