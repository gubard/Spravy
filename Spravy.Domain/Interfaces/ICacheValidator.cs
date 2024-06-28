namespace Spravy.Domain.Interfaces;

public interface ICacheValidator<in TKey, in TValue>
{
    bool IsValid(TKey key, TValue value);
}
