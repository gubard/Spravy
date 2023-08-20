namespace Spravy.Domain.Interfaces;

public interface IFactory<in TKey, out TValue> where TValue : notnull
{
    TValue Create(TKey key);
}