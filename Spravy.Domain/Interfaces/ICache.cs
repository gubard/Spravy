namespace Spravy.Domain.Interfaces;

public interface ICache<in TKey, TValue>
{
    bool TryGetCacheValue(TKey key, [MaybeNullWhen(false)] out TValue value);
}
