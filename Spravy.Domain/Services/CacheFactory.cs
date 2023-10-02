using System.Diagnostics.CodeAnalysis;
using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Services;

public class CacheFactory<TKey, TValue> : IFactory<TKey, TValue>, ICache<TKey, TValue>
    where TValue : notnull where TKey : notnull
{
    private static readonly Dictionary<TKey, TValue> cache = new();

    private readonly IFactory<TKey, TValue> factory;
    private readonly ICacheValidator<TKey, TValue> cacheValidator;

    public CacheFactory(IFactory<TKey, TValue> factory, ICacheValidator<TKey, TValue> cacheValidator)
    {
        this.factory = factory;
        this.cacheValidator = cacheValidator;
    }

    public TValue Create(TKey key)
    {
        if (!cache.TryGetValue(key, out var value))
        {
            return CreateNewValue(key);
        }

        if (cacheValidator.IsValid(key, value))
        {
            return value;
        }

        cache.Remove(key);

        return CreateNewValue(key);
    }

    private TValue CreateNewValue(TKey key)
    {
        var value = factory.Create(key);
        cache.Add(key, value);

        return value;
    }

    public bool TryGetCacheValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return cache.TryGetValue(key, out value);
    }
}