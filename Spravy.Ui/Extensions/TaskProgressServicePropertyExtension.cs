namespace Spravy.Ui.Extensions;

public static class TaskProgressServicePropertyExtension
{
    public static ConfiguredValueTaskAwaitable<Result> RunProgressAsync<TTaskProgressServiceProperty>(
        this TTaskProgressServiceProperty property,
        ushort impact,
        Func<TaskProgressItem, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken cancellationToken
    ) where TTaskProgressServiceProperty : ITaskProgressServiceProperty
    {
        return property.TaskProgressService
           .AddItemAsync(impact)
           .IfSuccessTryFinallyAsync(func, item => item.Finish(), cancellationToken);
    }
    
    public static ConfiguredValueTaskAwaitable<Result> RunProgressAsync<TTaskProgressServiceProperty>(
        this TTaskProgressServiceProperty property,
        Func<ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken cancellationToken
    ) where TTaskProgressServiceProperty : ITaskProgressServiceProperty
    {
        return property.TaskProgressService
           .AddItemAsync(1)
           .IfSuccessTryFinallyAsync(_ => func.Invoke(), item => item.Finish(), cancellationToken);
    }
}