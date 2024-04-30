using Spravy.Domain.Errors;
using Spravy.Domain.Models;

namespace Spravy.Domain.Helpers;

public static class CycleHelper
{
    public const ulong MaxCycleCount = ulong.MaxValue;

    public static Result While(Func<bool> predicate, Action action)
    {
        var count = 0ul;

        while (predicate.Invoke())
        {
            action.Invoke();
            count++;

            if (count == MaxCycleCount)
            {
                return new Result(new MaxCycleCountReachedError(MaxCycleCount));
            }
        }
        
        return Result.Success;
    }
}