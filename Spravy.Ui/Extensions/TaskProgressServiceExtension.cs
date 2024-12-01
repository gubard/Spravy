namespace Spravy.Ui.Extensions;

public static class TaskProgressServiceExtension
{
    public static ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TResult>>>
        RunProgressAsync<TTaskProgressServiceProperty, TItem, TResult>(
            this TTaskProgressServiceProperty property,
            ReadOnlyMemory<TItem> items,
            Func<TItem, ConfiguredValueTaskAwaitable<Result<TResult>>> func,
            CancellationToken ct
        ) where TTaskProgressServiceProperty : ITaskProgressService where TResult : notnull
    {
        return property.AddItem((ushort)items.Length, ct)
           .IfSuccessTryFinallyAsync(
                taskProgressItem =>
                    items.ToResult()
                       .IfSuccessForEachAsync(
                            item => func.Invoke(item)
                               .IfSuccessAsync(
                                    r => ct.PostUiBackground(taskProgressItem.IncreaseUi, ct)
                                       .IfSuccess(() => r.ToResult()),
                                    ct
                                ),
                            ct
                        ),
                item => item.Finish(),
                ct
            );
    }

    public static Cvtar RunProgressAsync<TTaskProgressServiceProperty, TItem>(
        this TTaskProgressServiceProperty property,
        ReadOnlyMemory<TItem> items,
        Func<TItem, Cvtar> func,
        CancellationToken ct
    ) where TTaskProgressServiceProperty : ITaskProgressService
    {
        return property.AddItem((ushort)items.Length, ct)
           .IfSuccessTryFinallyAsync(
                taskProgressItem =>
                    items.ToResult()
                       .IfSuccessForEachAsync(
                            item => func.Invoke(item)
                               .IfSuccessAsync(() => ct.PostUiBackground(taskProgressItem.IncreaseUi, ct), ct),
                            ct
                        ),
                item => item.Finish(),
                ct
            );
    }

    public static Result RunProgress<TTaskProgressServiceProperty, TItem>(
        this TTaskProgressServiceProperty property,
        ReadOnlyMemory<TItem> items,
        Func<TItem, Result> func,
        CancellationToken ct
    ) where TTaskProgressServiceProperty : ITaskProgressService
    {
        return property.AddItem((ushort)items.Length, ct)
           .IfSuccessTryFinally(
                taskProgressItem =>
                    items.ToResult()
                       .IfSuccessForEach(
                            item => func.Invoke(item)
                               .IfSuccess(() => property.PostUiBackground(taskProgressItem.IncreaseUi, ct))
                        ),
                item => item.Finish()
            );
    }

    public static Cvtar RunProgressAsync<TTaskProgressServiceProperty>(
        this TTaskProgressServiceProperty property,
        ushort impact,
        Func<TaskProgressItem, Cvtar> func,
        CancellationToken ct
    ) where TTaskProgressServiceProperty : ITaskProgressService
    {
        return property.AddItem(impact, ct).IfSuccessTryFinallyAsync(func, item => item.Finish(), ct);
    }

    public static Result RunProgress<TTaskProgressServiceProperty>(
        this TTaskProgressServiceProperty property,
        ushort impact,
        Func<TaskProgressItem, Result> func,
        CancellationToken ct
    ) where TTaskProgressServiceProperty : ITaskProgressService
    {
        return property.AddItem(impact, ct).IfSuccessTryFinally(func, item => item.Finish());
    }

    public static Cvtar RunProgressAsync<TTaskProgressServiceProperty>(
        this TTaskProgressServiceProperty property,
        Func<CancellationToken, Cvtar> func,
        CancellationToken ct
    ) where TTaskProgressServiceProperty : ITaskProgressService
    {
        return property.AddItem(1, ct).IfSuccessTryFinallyAsync(_ => func.Invoke(ct), item => item.Finish(), ct);
    }

    public static ConfiguredValueTaskAwaitable<Result<TReturn>> RunProgressAsync<TTaskProgressServiceProperty, TReturn>(
        this TTaskProgressServiceProperty property,
        Func<CancellationToken, ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        CancellationToken ct
    ) where TTaskProgressServiceProperty : ITaskProgressService where TReturn : notnull
    {
        return property.AddItem(1, ct).IfSuccessTryFinallyAsync(_ => func.Invoke(ct), item => item.Finish(), ct);
    }

    public static Cvtar RunProgressAsync<TTaskProgressServiceProperty, TParam>(
        this TTaskProgressServiceProperty property,
        Func<TParam, CancellationToken, Cvtar> func,
        TParam param,
        CancellationToken ct
    ) where TTaskProgressServiceProperty : ITaskProgressService
    {
        return property.AddItem(1, ct).IfSuccessTryFinallyAsync(_ => func.Invoke(param, ct), item => item.Finish(), ct);
    }
}