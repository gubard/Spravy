using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Services;

public class StaticKeeper<TValue> : IKeeper<TValue>
{
    private static TValue? instance;

    public void Set(TValue value)
    {
        instance = value;
    }

    public TValue? Get()
    {
        return instance;
    }
}