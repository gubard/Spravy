namespace Spravy.Domain.Extensions;

public static class ResultExtension
{
    public static Result IfSuccessForEach<TValue>(
        this Result<ReadOnlyMemory<TValue>> values,
        Func<TValue, Result> func
    )
    {
        if (!values.TryGetValue(out var v))
        {
            return new(values.Errors);
        }

        var valuesArray = v.ToArray();

        for (var index = 0; index < valuesArray.Length; index++)
        {
            var value = valuesArray[index];
            var result = func.Invoke(value);

            if (result.IsHasError)
            {
                return new(result.Errors);
            }
        }

        return Result.Success;
    }

    public static Result<ReadOnlyMemory<TReturn>> IfSuccessForEach<TValue, TReturn>(
        this Result<ReadOnlyMemory<TValue>> values,
        Func<TValue, Result<TReturn>> func
    )
        where TReturn : notnull
    {
        if (!values.TryGetValue(out var v))
        {
            return new(values.Errors);
        }

        var array = new TReturn[v.Length];
        var valuesArray = v.ToArray();

        for (var index = 0; index < valuesArray.Length; index++)
        {
            var value = valuesArray[index];
            var result = func.Invoke(value);

            if (!result.TryGetValue(out var rv))
            {
                return new(result.Errors);
            }

            array[index] = rv;
        }

        return array.ToReadOnlyMemory().ToResult();
    }

    public static ConfiguredValueTaskAwaitable<
        Result<ReadOnlyMemory<TReturn>>
    > IfSuccessForEachAsync<TValue, TReturn>(
        this ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TValue>>> task,
        Func<TValue, Result<TReturn>> func,
        CancellationToken ct
    )
        where TReturn : notnull
    {
        return IfSuccessForEachCore(task, func, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result<ReadOnlyMemory<TReturn>>> IfSuccessForEachCore<
        TValue,
        TReturn
    >(
        this ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TValue>>> task,
        Func<TValue, Result<TReturn>> func,
        CancellationToken ct
    )
        where TReturn : notnull
    {
        var values = await task;

        if (ct.IsCancellationRequested)
        {
            return Result<ReadOnlyMemory<TReturn>>.CanceledByUserError;
        }

        if (!values.TryGetValue(out var v))
        {
            return new(values.Errors);
        }

        var array = new TReturn[v.Length];
        var valuesArray = v.ToArray();

        for (var index = 0; index < valuesArray.Length; index++)
        {
            var value = valuesArray[index];
            var result = func.Invoke(value);

            if (!result.TryGetValue(out var rv))
            {
                return new(result.Errors);
            }

            if (ct.IsCancellationRequested)
            {
                return Result<ReadOnlyMemory<TReturn>>.CanceledByUserError;
            }

            array[index] = rv;
        }

        return array.ToReadOnlyMemory().ToResult();
    }

    public static ConfiguredValueTaskAwaitable<
        Result<ReadOnlyMemory<TReturn>>
    > IfSuccessForEachAsync<TValue, TReturn>(
        this Result<ReadOnlyMemory<TValue>> values,
        Func<TValue, ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        CancellationToken ct
    )
        where TReturn : notnull
    {
        return IfSuccessForEachCore(values, func, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result<ReadOnlyMemory<TReturn>>> IfSuccessForEachCore<
        TValue,
        TReturn
    >(
        this Result<ReadOnlyMemory<TValue>> values,
        Func<TValue, ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        CancellationToken ct
    )
        where TReturn : notnull
    {
        if (!values.TryGetValue(out var v))
        {
            return new(values.Errors);
        }

        if (ct.IsCancellationRequested)
        {
            return Result<ReadOnlyMemory<TReturn>>.CanceledByUserError;
        }

        var array = new TReturn[v.Length];
        var valuesArray = v.ToArray();

        for (var index = 0; index < valuesArray.Length; index++)
        {
            var value = valuesArray[index];
            var result = await func.Invoke(value);

            if (!result.TryGetValue(out var rv))
            {
                return new(result.Errors);
            }

            if (ct.IsCancellationRequested)
            {
                return Result<ReadOnlyMemory<TReturn>>.CanceledByUserError;
            }

            array[index] = rv;
        }

        return array.ToReadOnlyMemory().ToResult();
    }

    public static ConfiguredValueTaskAwaitable<
        Result<ReadOnlyMemory<TReturn>>
    > IfSuccessForEachAsync<TValue, TReturn>(
        this ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TValue>>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        CancellationToken ct
    )
        where TReturn : notnull
    {
        return IfSuccessForEachCore(task, func, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result<ReadOnlyMemory<TReturn>>> IfSuccessForEachCore<
        TValue,
        TReturn
    >(
        this ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TValue>>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        CancellationToken ct
    )
        where TReturn : notnull
    {
        var values = await task;

        if (ct.IsCancellationRequested)
        {
            return Result<ReadOnlyMemory<TReturn>>.CanceledByUserError;
        }

        if (!values.TryGetValue(out var v))
        {
            return new(values.Errors);
        }

        var array = new TReturn[v.Length];
        var valuesArray = v.ToArray();

        for (var index = 0; index < valuesArray.Length; index++)
        {
            var value = valuesArray[index];
            var result = await func.Invoke(value);

            if (!result.TryGetValue(out var rv))
            {
                return new(result.Errors);
            }

            if (ct.IsCancellationRequested)
            {
                return Result<ReadOnlyMemory<TReturn>>.CanceledByUserError;
            }

            array[index] = rv;
        }

        return array.ToReadOnlyMemory().ToResult();
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessForEachAsync<TValue>(
        this Result<ReadOnlyMemory<TValue>> values,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken ct
    )
    {
        return IfSuccessForEachCore(values, func, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfSuccessForEachCore<TValue>(
        this Result<ReadOnlyMemory<TValue>> values,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken ct
    )
    {
        if (!values.TryGetValue(out var v))
        {
            return new(values.Errors);
        }

        if (ct.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }

        foreach (var value in v.ToArray())
        {
            var result = await func.Invoke(value);

            if (result.IsHasError)
            {
                return new(result.Errors);
            }

            if (ct.IsCancellationRequested)
            {
                return Result.CanceledByUserError;
            }
        }

        return Result.Success;
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessForEachAsync<TValue>(
        this ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TValue>>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken ct
    )
    {
        return IfSuccessForEachCore(task, func, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfSuccessForEachCore<TValue>(
        this ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TValue>>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken ct
    )
    {
        var values = await task;

        if (!values.TryGetValue(out var v))
        {
            return new(values.Errors);
        }

        foreach (var value in v.ToArray())
        {
            var result = await func.Invoke(value);

            if (result.IsHasError)
            {
                return new(result.Errors);
            }

            if (ct.IsCancellationRequested)
            {
                return Result.CanceledByUserError;
            }
        }

        return Result.Success;
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAsync<TValue>(
        this ConfiguredCancelableAsyncEnumerable<Result<TValue>> enumerable,
        Func<TValue, Result> func,
        CancellationToken ct
    )
        where TValue : notnull
    {
        return IfSuccessCore(enumerable, func, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfSuccessCore<TValue>(
        this ConfiguredCancelableAsyncEnumerable<Result<TValue>> enumerable,
        Func<TValue, Result> func,
        CancellationToken ct
    )
        where TValue : notnull
    {
        await foreach (var result in enumerable)
        {
            if (ct.IsCancellationRequested)
            {
                return Result.CanceledByUserError;
            }

            if (!result.TryGetValue(out var rv))
            {
                return new(result.Errors);
            }

            var item = func.Invoke(rv);

            if (item.IsHasError)
            {
                return item;
            }
        }

        return Result.Success;
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAllInOrderAsync<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, Func<ConfiguredValueTaskAwaitable<Result>>[]> funcs,
        CancellationToken ct
    )
        where TValue : notnull
    {
        return IfSuccessAllInOrderCore(task, ct, funcs).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfSuccessAllInOrderCore<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        CancellationToken ct,
        Func<TValue, Func<ConfiguredValueTaskAwaitable<Result>>[]> funcs
    )
        where TValue : notnull
    {
        var result = await task;

        if (!result.TryGetValue(out var rv))
        {
            return new(result.Errors);
        }

        if (ct.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }

        var awaitables = funcs.Invoke(rv);

        foreach (var awaitable in awaitables)
        {
            var r = await awaitable.Invoke();

            if (ct.IsCancellationRequested)
            {
                return Result.CanceledByUserError;
            }

            if (r.IsHasError)
            {
                return r;
            }
        }

        return Result.Success;
    }

    public static ConfiguredValueTaskAwaitable<Result> IfErrorsAsync<TValue>(
        this Result<TValue> result,
        Func<ReadOnlyMemory<Error>, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken ct
    )
        where TValue : notnull
    {
        if (ct.IsCancellationRequested)
        {
            return Result.CanceledByUserError.ToValueTaskResult().ConfigureAwait(false);
        }

        if (result.IsHasError)
        {
            return func.Invoke(result.Errors);
        }

        return Result.AwaitableSuccess;
    }

    public static ConfiguredValueTaskAwaitable<Result> IfErrorsAsync(
        this ConfiguredValueTaskAwaitable<Result> task,
        Func<ReadOnlyMemory<Error>, Result> func,
        CancellationToken ct
    )
    {
        return IfErrorsCore(task, func, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfErrorsCore(
        this ConfiguredValueTaskAwaitable<Result> task,
        Func<ReadOnlyMemory<Error>, Result> func,
        CancellationToken ct
    )
    {
        var result = await task;

        if (ct.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }

        if (result.IsHasError)
        {
            return func.Invoke(result.Errors);
        }

        return Result.Success;
    }

    public static Result ToResultOnly<TValue>(this Result<TValue> task)
        where TValue : notnull
    {
        if (task.TryGetValue(out _))
        {
            return Result.Success;
        }

        return new(task.Errors);
    }

    public static ConfiguredValueTaskAwaitable<Result> ToResultOnlyAsync<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task
    )
        where TValue : notnull
    {
        return ToResultOnlyCore(task).ConfigureAwait(false);
    }

    private static async ValueTask<Result> ToResultOnlyCore<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task
    )
        where TValue : notnull
    {
        var result = await task;

        if (result.IsHasError)
        {
            return new(result.Errors);
        }

        return Result.Success;
    }

    public static async ValueTask ThrowIfErrorAsync(this ConfiguredValueTaskAwaitable<Result> task)
    {
        var result = await task;

        if (result.IsHasError)
        {
            throw new ErrorsException(result.Errors);
        }
    }

    public static TValue ThrowIfError<TValue>(this Result<TValue> result)
        where TValue : notnull
    {
        if (!result.TryGetValue(out var rv))
        {
            throw new ErrorsException(result.Errors);
        }

        return rv;
    }

    public static void ThrowIfError(this Result result)
    {
        if (result.IsHasError)
        {
            throw new ErrorsException(result.Errors);
        }
    }

    public static string GetTitle(this Result result)
    {
        var stringBuilder = new StringBuilder();

        foreach (var validationResult in result.Errors.Span)
        {
            stringBuilder.Append(validationResult.Message);
            stringBuilder.Append(";");
        }

        return stringBuilder.ToString();
    }

    public static string GetTitle<TValue>(this Result<TValue> result)
        where TValue : notnull
    {
        var stringBuilder = new StringBuilder();

        foreach (var validationResult in result.Errors.Span)
        {
            stringBuilder.Append(validationResult.Message);
            stringBuilder.Append(";");
        }

        return stringBuilder.ToString();
    }

    public static async ValueTask<Result> ToValueTaskResultOnly(this Task task)
    {
        await task;

        return Result.Success;
    }

    public static ConfiguredValueTaskAwaitable<Result<TReturn>> IfSuccessAsync<TReturn>(
        this ConfiguredValueTaskAwaitable<Result> task,
        Func<ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        CancellationToken ct
    )
        where TReturn : notnull
    {
        return IfSuccessCore(task, func, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result<TReturn>> IfSuccessCore<TReturn>(
        ConfiguredValueTaskAwaitable<Result> task,
        Func<ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        CancellationToken ct
    )
        where TReturn : notnull
    {
        var result = await task;

        if (result.IsHasError)
        {
            return new(result.Errors);
        }

        if (ct.IsCancellationRequested)
        {
            return Result<TReturn>.CanceledByUserError;
        }

        return await func.Invoke();
    }

    private static async ValueTask<Result> IfSuccessCore<TValue, TArg>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Result<TArg> arg,
        Func<TValue, TArg, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken ct
    )
        where TValue : notnull
        where TArg : notnull
    {
        var result = await task;

        if (!result.TryGetValue(out var rv))
        {
            return new(result.Errors);
        }

        if (!arg.TryGetValue(out var a1))
        {
            return new(arg.Errors);
        }

        if (ct.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }

        return await func.Invoke(rv, a1);
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAsync<TValue, TArg>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Result<TArg> arg,
        Func<TValue, TArg, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken ct
    )
        where TValue : notnull
        where TArg : notnull
    {
        return IfSuccessCore(task, arg, func, ct).ConfigureAwait(false);
    }

    public static ConfiguredValueTaskAwaitable<Result<TReturn>> IfSuccessAsync<
        TValue,
        TArg,
        TReturn
    >(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Result<TArg> arg,
        Func<TValue, TArg, ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        CancellationToken ct
    )
        where TReturn : notnull
        where TArg : notnull
        where TValue : notnull
    {
        return IfSuccessCore(task, arg, func, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result<TReturn>> IfSuccessCore<TValue, TArg, TReturn>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Result<TArg> arg,
        Func<TValue, TArg, ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        CancellationToken ct
    )
        where TReturn : notnull
        where TValue : notnull
        where TArg : notnull
    {
        var result = await task;

        if (!result.TryGetValue(out var rv))
        {
            return new(result.Errors);
        }

        if (!arg.TryGetValue(out var a1))
        {
            return new(arg.Errors);
        }

        if (ct.IsCancellationRequested)
        {
            return Result<TReturn>.CanceledByUserError;
        }

        return await func.Invoke(rv, a1);
    }

    public static ConfiguredValueTaskAwaitable<Result<TReturn>> IfSuccessAsync<
        TValue,
        TArg,
        TReturn
    >(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Result<TArg> arg,
        Func<TValue, TArg, ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        Func<ReadOnlyMemory<Error>, ConfiguredValueTaskAwaitable<Result<TReturn>>> errorsFunc,
        CancellationToken ct
    )
        where TReturn : notnull
        where TArg : notnull
        where TValue : notnull
    {
        return IfSuccessCore(task, arg, func, errorsFunc, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result<TReturn>> IfSuccessCore<TValue, TArg, TReturn>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Result<TArg> arg,
        Func<TValue, TArg, ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        Func<ReadOnlyMemory<Error>, ConfiguredValueTaskAwaitable<Result<TReturn>>> errorsFunc,
        CancellationToken ct
    )
        where TReturn : notnull
        where TArg : notnull
        where TValue : notnull
    {
        var result = await task;

        if (!result.TryGetValue(out var rv))
        {
            return new(result.Errors);
        }

        if (!arg.TryGetValue(out var a1))
        {
            return new(arg.Errors);
        }

        if (ct.IsCancellationRequested)
        {
            return Result<TReturn>.CanceledByUserError;
        }

        return await func.Invoke(rv, a1);
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAsync<TValue, TArg1, TArg2>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Result<TArg1> arg1,
        Result<TArg2> arg2,
        Func<TValue, TArg1, TArg2, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken ct
    )
        where TValue : notnull
        where TArg1 : notnull
        where TArg2 : notnull
    {
        return IfSuccessCore(task, arg1, arg2, func, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfSuccessCore<TValue, TArg1, TArg2>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Result<TArg1> arg1,
        Result<TArg2> arg2,
        Func<TValue, TArg1, TArg2, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken ct
    )
        where TValue : notnull
        where TArg1 : notnull
        where TArg2 : notnull
    {
        var result = await task;

        if (!result.TryGetValue(out var rv))
        {
            return new(result.Errors);
        }

        if (!arg1.TryGetValue(out var a1))
        {
            return new(arg1.Errors);
        }

        if (!arg2.TryGetValue(out var a2))
        {
            return new(arg2.Errors);
        }

        if (ct.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }

        return await func.Invoke(rv, a1, a2);
    }

    public static ConfiguredValueTaskAwaitable<Result<TReturn>> IfSuccessAsync<TReturn>(
        this ConfiguredValueTaskAwaitable<Result> task,
        Func<Result<TReturn>> func,
        CancellationToken ct
    )
        where TReturn : notnull
    {
        return IfSuccessCore(task, func, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result<TReturn>> IfSuccessCore<TReturn>(
        this ConfiguredValueTaskAwaitable<Result> task,
        Func<Result<TReturn>> func,
        CancellationToken ct
    )
        where TReturn : notnull
    {
        var result = await task;

        if (result.IsHasError)
        {
            return new(result.Errors);
        }

        if (ct.IsCancellationRequested)
        {
            return Result<TReturn>.CanceledByUserError;
        }

        return func.Invoke();
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAsync(
        this ConfiguredValueTaskAwaitable<Result> task,
        Func<Result> func,
        CancellationToken ct
    )
    {
        return IfSuccessCore(task, func, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfSuccessCore(
        this ConfiguredValueTaskAwaitable<Result> task,
        Func<Result> func,
        CancellationToken ct
    )
    {
        var result = await task;

        if (result.IsHasError)
        {
            return new(result.Errors);
        }

        if (ct.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }

        return func.Invoke();
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAsync(
        this ConfiguredValueTaskAwaitable<Result> task,
        Func<ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken ct
    )
    {
        return IfSuccessCore(task, func, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfSuccessCore(
        this ConfiguredValueTaskAwaitable<Result> task,
        Func<ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken ct
    )
    {
        var result = await task;

        if (result.IsHasError)
        {
            return new(result.Errors);
        }

        if (ct.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }

        return await func.Invoke();
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAllAsync(
        this ConfiguredValueTaskAwaitable<Result> task,
        CancellationToken ct,
        params Func<ConfiguredValueTaskAwaitable<Result>>[] funcs
    )
    {
        return IfSuccessAllCore(task, ct, funcs).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfSuccessAllCore(
        this ConfiguredValueTaskAwaitable<Result> task,
        CancellationToken ct,
        params Func<ConfiguredValueTaskAwaitable<Result>>[] funcs
    )
    {
        var result = await task;

        if (result.IsHasError)
        {
            return new(result.Errors);
        }

        if (ct.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }

        var tasks = funcs.Select(x => x.Invoke()).ToArray();
        var errors = ReadOnlyMemory<Error>.Empty;

        foreach (var awaitable in tasks)
        {
            var r = await awaitable;

            if (ct.IsCancellationRequested)
            {
                return Result.CanceledByUserError;
            }

            errors = errors.Combine(r.Errors);
        }

        if (errors.IsEmpty)
        {
            return Result.Success;
        }

        return new(errors);
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAllAsync<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        CancellationToken cancellationToke,
        params Func<TValue, ConfiguredValueTaskAwaitable<Result>>[] funcs
    )
        where TValue : notnull
    {
        return IfSuccessAllCore(task, cancellationToke, funcs).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfSuccessAllCore<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        CancellationToken ct,
        params Func<TValue, ConfiguredValueTaskAwaitable<Result>>[] funcs
    )
        where TValue : notnull
    {
        var result = await task;

        if (!result.TryGetValue(out var rv))
        {
            return new(result.Errors);
        }

        if (ct.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }

        var tasks = funcs.Select(x => x.Invoke(rv)).ToArray();
        var errors = ReadOnlyMemory<Error>.Empty;

        foreach (var awaitable in tasks)
        {
            var r = await awaitable;

            if (ct.IsCancellationRequested)
            {
                return Result.CanceledByUserError;
            }

            errors = errors.Combine(r.Errors);
        }

        return new(errors);
    }

    public static ConfiguredValueTaskAwaitable<Result<TReturn>> IfSuccessAsync<TReturn>(
        this Result result,
        Func<ConfiguredValueTaskAwaitable<Result<TReturn>>> action,
        CancellationToken ct
    )
        where TReturn : notnull
    {
        return IfSuccessCore(result, action, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result<TReturn>> IfSuccessCore<TReturn>(
        this Result result,
        Func<ConfiguredValueTaskAwaitable<Result<TReturn>>> action,
        CancellationToken ct
    )
        where TReturn : notnull
    {
        if (result.IsHasError)
        {
            return new(result.Errors);
        }

        if (ct.IsCancellationRequested)
        {
            return Result<TReturn>.CanceledByUserError;
        }

        return await action.Invoke();
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAsync(
        this Result result,
        Func<ConfiguredValueTaskAwaitable<Result>> action,
        CancellationToken ct
    )
    {
        return IfSuccessCore(result, action, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfSuccessCore(
        this Result result,
        Func<ConfiguredValueTaskAwaitable<Result>> action,
        CancellationToken ct
    )
    {
        if (result.IsHasError)
        {
            return new(result.Errors);
        }

        if (ct.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }

        return await action.Invoke();
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAsync<TValue>(
        this Result<TValue> result,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> action,
        Func<ReadOnlyMemory<Error>, ConfiguredValueTaskAwaitable<Result>> errors,
        CancellationToken ct
    )
        where TValue : notnull
    {
        if (!result.TryGetValue(out var rv))
        {
            return errors.Invoke(result.Errors);
        }

        if (ct.IsCancellationRequested)
        {
            return Result.AwaitableCanceledByUserError;
        }

        return action.Invoke(rv);
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAsync<TValue>(
        this Result<TValue> result,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> action,
        CancellationToken ct
    )
        where TValue : notnull
    {
        return IfSuccessCore(result, action, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfSuccessCore<TValue>(
        this Result<TValue> result,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> action,
        CancellationToken ct
    )
        where TValue : notnull
    {
        if (!result.TryGetValue(out var rv))
        {
            return new(result.Errors);
        }

        if (ct.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }

        return await action.Invoke(rv);
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAsync<TValue, TArg>(
        this Result<TValue> result,
        Result<TArg> arg,
        Func<TValue, TArg, ConfiguredValueTaskAwaitable<Result>> action,
        CancellationToken ct
    )
        where TValue : notnull
        where TArg : notnull
    {
        if (!result.TryGetValue(out var rv))
        {
            return new Result(result.Errors).ToValueTaskResult().ConfigureAwait(false);
        }

        if (!arg.TryGetValue(out var a))
        {
            return new Result(arg.Errors).ToValueTaskResult().ConfigureAwait(false);
        }

        if (ct.IsCancellationRequested)
        {
            return Result.CanceledByUserError.ToValueTaskResult().ConfigureAwait(false);
        }

        return action.Invoke(rv, a);
    }

    public static ConfiguredValueTaskAwaitable<Result<TReturn>> IfSuccessAsync<TValue, TReturn>(
        this Result<TValue> result,
        Func<TValue, ConfiguredValueTaskAwaitable<Result<TReturn>>> action,
        CancellationToken ct
    )
        where TReturn : notnull
        where TValue : notnull
    {
        if (!result.TryGetValue(out var rv))
        {
            return new Result<TReturn>(result.Errors).ToValueTaskResult().ConfigureAwait(false);
        }

        if (ct.IsCancellationRequested)
        {
            return Result<TReturn>.CanceledByUserError.ToValueTaskResult().ConfigureAwait(false);
        }

        return action.Invoke(rv);
    }

    public static ConfiguredValueTaskAwaitable<Result<TReturn>> IfSuccessAsync<TValue, TReturn>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result<TReturn>>> action,
        CancellationToken ct
    )
        where TReturn : notnull
        where TValue : notnull
    {
        return IfSuccessCore(task, action, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result<TReturn>> IfSuccessCore<TValue, TReturn>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result<TReturn>>> action,
        CancellationToken ct
    )
        where TReturn : notnull
        where TValue : notnull
    {
        var result = await task;

        if (!result.TryGetValue(out var rv))
        {
            return new(result.Errors);
        }

        if (ct.IsCancellationRequested)
        {
            return Result<TReturn>.CanceledByUserError;
        }

        return await action.Invoke(rv);
    }

    public static ConfiguredValueTaskAwaitable<Result<TReturn>> IfSuccessAsync<TValue, TReturn>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        Func<ReadOnlyMemory<Error>, ConfiguredValueTaskAwaitable<Result<TReturn>>> errorsFunc,
        CancellationToken ct
    )
        where TReturn : notnull
        where TValue : notnull
    {
        return IfSuccessCore(task, func, errorsFunc, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result<TReturn>> IfSuccessCore<TValue, TReturn>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        Func<ReadOnlyMemory<Error>, ConfiguredValueTaskAwaitable<Result<TReturn>>> errorsFunc,
        CancellationToken ct
    )
        where TReturn : notnull
        where TValue : notnull
    {
        var result = await task;

        if (!result.TryGetValue(out var rv))
        {
            return await errorsFunc.Invoke(result.Errors);
        }

        if (ct.IsCancellationRequested)
        {
            return Result<TReturn>.CanceledByUserError;
        }

        return await func.Invoke(rv);
    }

    public static ConfiguredValueTaskAwaitable<Result<TReturn>> IfSuccessAsync<TValue, TReturn>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, Result<TReturn>> func,
        CancellationToken ct
    )
        where TReturn : notnull
        where TValue : notnull
    {
        return IfSuccessCore(task, func, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result<TReturn>> IfSuccessCore<TValue, TReturn>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, Result<TReturn>> func,
        CancellationToken ct
    )
        where TReturn : notnull
        where TValue : notnull
    {
        var result = await task;

        if (!result.TryGetValue(out var rv))
        {
            return new(result.Errors);
        }

        if (ct.IsCancellationRequested)
        {
            return Result<TReturn>.CanceledByUserError;
        }

        return func.Invoke(rv);
    }

    public static ConfiguredValueTaskAwaitable<Result<TReturn>> IfSuccessDisposeAsync<
        TValue,
        TReturn
    >(
        this Result<TValue> result,
        Func<TValue, ConfiguredValueTaskAwaitable<Result<TReturn>>> action,
        CancellationToken ct
    )
        where TValue : IAsyncDisposable
        where TReturn : notnull
    {
        return IfSuccessDisposeCore(result, action, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result<TReturn>> IfSuccessDisposeCore<TValue, TReturn>(
        this Result<TValue> result,
        Func<TValue, ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        CancellationToken ct
    )
        where TValue : IAsyncDisposable
        where TReturn : notnull
    {
        if (!result.TryGetValue(out var rv))
        {
            return new(result.Errors);
        }

        await using var value = rv;

        if (ct.IsCancellationRequested)
        {
            return Result<TReturn>.CanceledByUserError;
        }

        return await func.Invoke(value);
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessDisposeAsync<TValue>(
        this Result<TValue> result,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> action,
        CancellationToken ct
    )
        where TValue : IAsyncDisposable
    {
        return IfSuccessDisposeCore(result, action, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfSuccessDisposeCore<TValue>(
        this Result<TValue> result,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken ct
    )
        where TValue : IAsyncDisposable
    {
        if (!result.TryGetValue(out var rv))
        {
            return new(result.Errors);
        }

        await using var value = rv;

        if (ct.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }

        return await func.Invoke(value);
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAsync<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, Result> action,
        CancellationToken ct
    )
        where TValue : notnull
    {
        return IfSuccessCore(task, action, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfSuccessCore<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, Result> action,
        CancellationToken ct
    )
        where TValue : notnull
    {
        var result = await task;

        if (!result.TryGetValue(out var rv))
        {
            return new(result.Errors);
        }

        if (ct.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }

        return action.Invoke(rv);
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAsync<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> action,
        CancellationToken ct
    )
        where TValue : notnull
    {
        return IfSuccessCore(task, action, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfSuccessCore<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> action,
        CancellationToken ct
    )
        where TValue : notnull
    {
        var result = await task;

        if (!result.TryGetValue(out var rv))
        {
            return new(result.Errors);
        }

        if (ct.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }

        return await action.Invoke(rv);
    }

    public static Result<TReturn> IfSuccess<TReturn>(
        this Result result,
        Func<Result<TReturn>> action
    )
        where TReturn : notnull
    {
        if (result.IsHasError)
        {
            return new(result.Errors);
        }

        return action.Invoke();
    }

    public static Result<TReturn> IfSuccess<TValue, TReturn>(
        this Result<TValue> result,
        Func<TValue, Result<TReturn>> action
    )
        where TReturn : notnull
        where TValue : notnull
    {
        if (!result.TryGetValue(out var rv))
        {
            return new(result.Errors);
        }

        return action.Invoke(rv);
    }

    public static Result IfSuccess<TValue>(this Result<TValue> result, Func<TValue, Result> action)
        where TValue : notnull
    {
        if (!result.TryGetValue(out var rv))
        {
            return new(result.Errors);
        }

        return action.Invoke(rv);
    }

    public static Result IfSuccess(this Result result, Func<Result> action)
    {
        if (result.IsHasError)
        {
            return new(result.Errors);
        }

        return action.Invoke();
    }

    public static Result<TReturn> IfSuccess<TArg1, TArg2, TReturn>(
        this Result result,
        Result<TArg1> arg1,
        Result<TArg2> arg2,
        Func<TArg1, TArg2, Result<TReturn>> action
    )
        where TReturn : notnull
        where TArg1 : notnull
        where TArg2 : notnull
    {
        if (result.IsHasError)
        {
            return new(result.Errors);
        }

        if (!arg1.TryGetValue(out var a1))
        {
            return new(arg1.Errors);
        }

        if (!arg2.TryGetValue(out var a2))
        {
            return new(arg2.Errors);
        }

        return action.Invoke(a1, a2);
    }

    public static Result<TReturn> IfSuccess<TValue, TArg1, TArg2, TReturn>(
        this Result<TValue> result,
        Result<TArg1> arg1,
        Result<TArg2> arg2,
        Func<TValue, TArg1, TArg2, Result<TReturn>> action
    )
        where TReturn : notnull
        where TArg1 : notnull
        where TArg2 : notnull
        where TValue : notnull
    {
        if (!result.TryGetValue(out var rv))
        {
            return new(result.Errors);
        }

        if (!arg1.TryGetValue(out var a1))
        {
            return new(arg1.Errors);
        }

        if (!arg2.TryGetValue(out var a2))
        {
            return new(arg2.Errors);
        }

        return action.Invoke(rv, a1, a2);
    }

    public static ConfiguredValueTaskAwaitable<Result<TResult>> IfSuccessTryFinallyAsync<
        TValue,
        TResult
    >(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result<TResult>>> funcTry,
        Action<TValue> funcFinally,
        CancellationToken ct
    )
        where TValue : notnull
        where TResult : notnull
    {
        return IfSuccessTryFinallyCore(task, funcTry, funcFinally, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result<TResult>> IfSuccessTryFinallyCore<TValue, TResult>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result<TResult>>> funcTry,
        Action<TValue> funcFinally,
        CancellationToken ct
    )
        where TValue : notnull
        where TResult : notnull
    {
        var result = await task;

        if (!result.TryGetValue(out var rv))
        {
            return new(result.Errors);
        }

        try
        {
            if (ct.IsCancellationRequested)
            {
                return Result<TResult>.CanceledByUserError;
            }

            return await funcTry.Invoke(rv);
        }
        finally
        {
            funcFinally.Invoke(rv);
        }
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessTryFinallyAsync<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> funcTry,
        Action<TValue> funcFinally,
        CancellationToken ct
    )
        where TValue : notnull
    {
        return IfSuccessTryFinallyCore(task, funcTry, funcFinally, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfSuccessTryFinallyCore<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> funcTry,
        Action<TValue> funcFinally,
        CancellationToken ct
    )
        where TValue : notnull
    {
        var result = await task;

        if (!result.TryGetValue(out var rv))
        {
            return new(result.Errors);
        }

        try
        {
            if (ct.IsCancellationRequested)
            {
                return Result.CanceledByUserError;
            }

            return await funcTry.Invoke(rv);
        }
        finally
        {
            funcFinally.Invoke(rv);
        }
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessTryFinallyAsync<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> funcTry,
        Func<TValue, ConfiguredValueTaskAwaitable> funcFinally,
        CancellationToken ct
    )
        where TValue : notnull
    {
        return IfSuccessTryFinallyCore(task, funcTry, funcFinally, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfSuccessTryFinallyCore<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> funcTry,
        Func<TValue, ConfiguredValueTaskAwaitable> funcFinally,
        CancellationToken ct
    )
        where TValue : notnull
    {
        var result = await task;

        if (!result.TryGetValue(out var rv))
        {
            return new(result.Errors);
        }

        try
        {
            if (ct.IsCancellationRequested)
            {
                return Result.CanceledByUserError;
            }

            return await funcTry.Invoke(rv);
        }
        finally
        {
            await funcFinally.Invoke(rv);
        }
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessTryFinallyAsync(
        this ConfiguredValueTaskAwaitable<Result> task,
        Func<ConfiguredValueTaskAwaitable<Result>> funcTry,
        Func<ConfiguredValueTaskAwaitable> funcFinally,
        CancellationToken ct
    )
    {
        return IfSuccessTryFinallyCore(task, funcTry, funcFinally, ct).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfSuccessTryFinallyCore(
        this ConfiguredValueTaskAwaitable<Result> task,
        Func<ConfiguredValueTaskAwaitable<Result>> funcTry,
        Func<ConfiguredValueTaskAwaitable> funcFinally,
        CancellationToken ct
    )
    {
        try
        {
            var result = await task;

            if (result.IsHasError)
            {
                return new(result.Errors);
            }

            if (ct.IsCancellationRequested)
            {
                return Result.CanceledByUserError;
            }

            return await funcTry.Invoke();
        }
        finally
        {
            await funcFinally.Invoke();
        }
    }

    public static async ValueTask ToValueTask(this ConfiguredValueTaskAwaitable<Result> task)
    {
        await task;
    }
}
