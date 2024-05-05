namespace Spravy.Ui.Extensions;

public static class TaskProgressServicePropertyExtension
{
    public static ConfiguredValueTaskAwaitable<Result> RunProgressAsync<TTaskProgressServiceProperty>(
        this TTaskProgressServiceProperty property,
        double impact,
        Func<TaskProgressItem, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken cancellationToken
    ) where TTaskProgressServiceProperty : ITaskProgressServiceProperty
    {
        return property.TaskProgressService
           .AddItemAsync(impact)
           .IfSuccessTryFinallyAsync(func,
                item => property.TaskProgressService.DeleteItemAsync(item).ToValueTask().ConfigureAwait(false),
                cancellationToken);
    }
    
    public static ConfiguredValueTaskAwaitable<Result> RunProgressAsync<TTaskProgressServiceProperty>(
        this TTaskProgressServiceProperty property,
        Func<ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken cancellationToken
    ) where TTaskProgressServiceProperty : ITaskProgressServiceProperty
    {
        return property.TaskProgressService
           .AddItemAsync(1)
           .IfSuccessTryFinallyAsync(_ => func.Invoke(),
                item => property.TaskProgressService.DeleteItemAsync(item).ToValueTask().ConfigureAwait(false),
                cancellationToken);
    }
}