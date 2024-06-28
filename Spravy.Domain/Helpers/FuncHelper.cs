namespace Spravy.Domain.Helpers;

public class FuncHelper<T>
{
    public static readonly Func<T, Task> CompletedTask = _ => Task.CompletedTask;
}
