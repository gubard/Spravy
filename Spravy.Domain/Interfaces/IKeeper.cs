namespace Spravy.Domain.Interfaces;

public interface IKeeper<TValue>
{
    void Set(TValue value);
    TValue? Get();
}