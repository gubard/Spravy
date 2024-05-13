namespace Spravy.Ui.Extensions;

public static class TaskProgressServiceExtension
{
    public static ConfiguredValueTaskAwaitable<Result> RunProgressAsync<TTaskProgressServiceProperty>(
        this TTaskProgressServiceProperty property,
        ushort impact,
        Func<TaskProgressItem, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken cancellationToken
    ) where TTaskProgressServiceProperty : ITaskProgressService
    {
        return property.AddItemAsync(impact).IfSuccessTryFinallyAsync(func, item => item.Finish(), cancellationToken);
    }
    
    public static ConfiguredValueTaskAwaitable<Result> RunProgressAsync<TTaskProgressServiceProperty>(
        this TTaskProgressServiceProperty property,
        Func<ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken cancellationToken
    ) where TTaskProgressServiceProperty : ITaskProgressService
    {
        return property.AddItemAsync(1)
           .IfSuccessTryFinallyAsync(_ => func.Invoke(), item => item.Finish(), cancellationToken);
    }
}