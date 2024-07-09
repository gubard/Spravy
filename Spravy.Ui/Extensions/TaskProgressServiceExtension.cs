namespace Spravy.Ui.Extensions;

public static class TaskProgressServiceExtension
{
    public static ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TResult>>> RunProgressAsync<
        TTaskProgressServiceProperty,
        TItem,
        TResult
    >(
        this TTaskProgressServiceProperty property,
        ReadOnlyMemory<TItem> items,
        Func<TItem, ConfiguredValueTaskAwaitable<Result<TResult>>> func,
        CancellationToken ct
    )
        where TTaskProgressServiceProperty : ITaskProgressService
        where TResult : notnull
    {
        return property
            .AddItem((ushort)items.Length)
            .IfSuccessTryFinallyAsync(
                taskProgressItem =>
                    items
                        .ToResult()
                        .IfSuccessForEachAsync(
                            item =>
                                func.Invoke(item)
                                    .IfSuccessAsync(
                                        r =>
                                            taskProgressItem
                                                .Increase()
                                                .IfSuccess(() => r.ToResult()),
                                        ct
                                    ),
                            ct
                        ),
                item => item.Finish(),
                ct
            );
    }

    public static ConfiguredValueTaskAwaitable<Result> RunProgressAsync<
        TTaskProgressServiceProperty,
        TItem
    >(
        this TTaskProgressServiceProperty property,
        ReadOnlyMemory<TItem> items,
        Func<TItem, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken ct
    )
        where TTaskProgressServiceProperty : ITaskProgressService
    {
        return property
            .AddItem((ushort)items.Length)
            .IfSuccessTryFinallyAsync(
                taskProgressItem =>
                    items
                        .ToResult()
                        .IfSuccessForEachAsync(
                            item => func.Invoke(item).IfSuccessAsync(taskProgressItem.Increase, ct),
                            ct
                        ),
                item => item.Finish(),
                ct
            );
    }

    public static Result RunProgress<TTaskProgressServiceProperty, TItem>(
        this TTaskProgressServiceProperty property,
        ReadOnlyMemory<TItem> items,
        Func<TItem, Result> func
    )
        where TTaskProgressServiceProperty : ITaskProgressService
    {
        return property
            .AddItem((ushort)items.Length)
            .IfSuccessTryFinally(
                taskProgressItem =>
                    items
                        .ToResult()
                        .IfSuccessForEach(item =>
                            func.Invoke(item).IfSuccess(taskProgressItem.Increase)
                        ),
                item => item.Finish()
            );
    }

    public static ConfiguredValueTaskAwaitable<Result> RunProgressAsync<TTaskProgressServiceProperty>(
        this TTaskProgressServiceProperty property,
        ushort impact,
        Func<TaskProgressItem, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken ct
    )
        where TTaskProgressServiceProperty : ITaskProgressService
    {
        return property.AddItem(impact).IfSuccessTryFinallyAsync(func, item => item.Finish(), ct);
    }

    public static Result RunProgress<TTaskProgressServiceProperty>(
        this TTaskProgressServiceProperty property,
        ushort impact,
        Func<TaskProgressItem, Result> func
    )
        where TTaskProgressServiceProperty : ITaskProgressService
    {
        return property.AddItem(impact).IfSuccessTryFinally(func, item => item.Finish());
    }

    public static ConfiguredValueTaskAwaitable<Result> RunProgressAsync<TTaskProgressServiceProperty>(
        this TTaskProgressServiceProperty property,
        Func<CancellationToken, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken ct
    )
        where TTaskProgressServiceProperty : ITaskProgressService
    {
        return property
            .AddItem(1)
            .IfSuccessTryFinallyAsync(_ => func.Invoke(ct), item => item.Finish(), ct);
    }

    public static ConfiguredValueTaskAwaitable<Result> RunProgressAsync<
        TTaskProgressServiceProperty,
        TParam
    >(
        this TTaskProgressServiceProperty property,
        Func<TParam, CancellationToken, ConfiguredValueTaskAwaitable<Result>> func,
        TParam param,
        CancellationToken ct
    )
        where TTaskProgressServiceProperty : ITaskProgressService
    {
        return property
            .AddItem(1)
            .IfSuccessTryFinallyAsync(_ => func.Invoke(param, ct), item => item.Finish(), ct);
    }
}
