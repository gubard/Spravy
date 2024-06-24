namespace Spravy.Ui.Extensions;

public static class TaskProgressServiceExtension
{
    public static ConfiguredValueTaskAwaitable<Result> RunProgressAsync<TTaskProgressServiceProperty>(
        this TTaskProgressServiceProperty property,
        ushort impact,
        Func<TaskProgressItem, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken ct
    ) where TTaskProgressServiceProperty : ITaskProgressService
    {
        return property.AddItemAsync(impact).IfSuccessTryFinallyAsync(func, item => item.Finish(), ct);
    }
    
    public static ConfiguredValueTaskAwaitable<Result> RunProgressAsync<TTaskProgressServiceProperty>(
        this TTaskProgressServiceProperty property,
        Func<CancellationToken, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken ct
    ) where TTaskProgressServiceProperty : ITaskProgressService
    {
        return property.AddItemAsync(1)
           .IfSuccessTryFinallyAsync(_ => func.Invoke(ct), item => item.Finish(), ct);
    }
    
    public static ConfiguredValueTaskAwaitable<Result> RunProgressAsync<TTaskProgressServiceProperty, TParam>(
        this TTaskProgressServiceProperty property,
        Func<TParam, CancellationToken, ConfiguredValueTaskAwaitable<Result>> func,
        TParam param,
        CancellationToken ct
    ) where TTaskProgressServiceProperty : ITaskProgressService
    {
        return property.AddItemAsync(1)
           .IfSuccessTryFinallyAsync(_ => func.Invoke(param, ct), item => item.Finish(), ct);
    }
}