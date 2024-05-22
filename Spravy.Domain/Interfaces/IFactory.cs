namespace Spravy.Domain.Interfaces;

public interface IFactory<in TKey, TValue> where TValue : notnull
{
    Result<TValue> Create(TKey key);
}

public interface IFactory<TValue> where TValue : notnull
{
    Result<TValue> Create();
}