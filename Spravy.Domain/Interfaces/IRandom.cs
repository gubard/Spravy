namespace Spravy.Domain.Interfaces;

public interface IRandom<out TValue>
{
    TValue? GetRandom();
}

public interface IRandom<out TValue, in TOptions>
{
    TValue? GetRandom(TOptions options);
}
