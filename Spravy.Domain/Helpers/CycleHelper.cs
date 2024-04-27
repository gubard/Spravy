namespace Spravy.Domain.Helpers;

public static class CycleHelper
{
    public const ulong MaxCycleCount = ulong.MaxValue;

    public static void While(Func<bool> predicate, Action action)
    {
        var count = 0ul;

        while (predicate.Invoke())
        {
            action.Invoke();
            count++;

            if (count == MaxCycleCount)
            {
                throw new Exception("Max cycle count reached");
            }
        }
    }
}