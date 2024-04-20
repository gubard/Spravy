using System.Runtime.CompilerServices;
using Spravy.Domain.Errors;
using Spravy.Domain.Models;

namespace Spravy.Domain.Extensions;

public static class ResultExtension
{
    public static async ValueTask<Result> ToValueTaskResultOnly(this Task task)
    {
        await task;

        return Result.Success;
    }

    public static ConfiguredValueTaskAwaitable<Result<TReturn>> IfSuccessAsync<TReturn>(
        this ConfiguredValueTaskAwaitable<Result> task,
        Func<ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        CancellationToken cancellationToken
    )
    {
        return IfSuccessCore(task, func, cancellationToken).ConfigureAwait(false);
    }

    private static async ValueTask<Result<TReturn>> IfSuccessCore<TReturn>(
        ConfiguredValueTaskAwaitable<Result> task,
        Func<ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        CancellationToken cancellationToken
    )
    {
        var result = await task;

        if (result.IsHasError)
        {
            return new Result<TReturn>(result.Errors);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result<TReturn>.CanceledByUserError;
        }

        return await func.Invoke();
    }

    private static async ValueTask<Result> IfSuccessCore<TValue, TArg>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Result<TArg> arg,
        Func<TValue, TArg, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken cancellationToken
    )
    {
        var result = await task;
        var errors = arg.Errors.Combine(result.Errors);

        if (!errors.IsEmpty)
        {
            return new Result(errors);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }

        return await func.Invoke(result.Value, arg.Value);
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAsync<TValue, TArg>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Result<TArg> arg,
        Func<TValue, TArg, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken cancellationToken
    )
    {
        return IfSuccessCore(task, arg, func, cancellationToken).ConfigureAwait(false);
    }

    public static ConfiguredValueTaskAwaitable<Result<TReturn>> IfSuccessAsync<TValue, TArg, TReturn>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Result<TArg> arg,
        Func<TValue, TArg, ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        CancellationToken cancellationToken
    )
    {
        return IfSuccessCore(task, arg, func, cancellationToken).ConfigureAwait(false);
    }

    private static async ValueTask<Result<TReturn>> IfSuccessCore<TValue, TArg, TReturn>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Result<TArg> arg,
        Func<TValue, TArg, ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        CancellationToken cancellationToken
    )
    {
        var result = await task;
        var errors = arg.Errors.Combine(result.Errors);

        if (!errors.IsEmpty)
        {
            return new Result<TReturn>(errors);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result<TReturn>.CanceledByUserError;
        }

        return await func.Invoke(result.Value, arg.Value);
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAsync<TValue, TArg1, TArg2>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Result<TArg1> arg1,
        Result<TArg2> arg2,
        Func<TValue, TArg1, TArg2, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken cancellationToken
    )
    {
        return IfSuccessCore(task, arg1, arg2, func, cancellationToken).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfSuccessCore<TValue, TArg1, TArg2>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Result<TArg1> arg1,
        Result<TArg2> arg2,
        Func<TValue, TArg1, TArg2, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken cancellationToken
    )
    {
        var result = await task;
        var errors = arg1.Errors.Combine(arg2.Errors, result.Errors);

        if (!errors.IsEmpty)
        {
            return new Result(errors);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }

        return await func.Invoke(result.Value, arg1.Value, arg2.Value);
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAsync(
        this ConfiguredValueTaskAwaitable<Result> task,
        Func<ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken cancellationToken
    )
    {
        return IfSuccessCore(task, func, cancellationToken).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfSuccessCore(
        this ConfiguredValueTaskAwaitable<Result> task,
        Func<ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken cancellationToken
    )
    {
        var result = await task;

        if (result.IsHasError)
        {
            return new Result(result.Errors);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }

        return await func.Invoke();
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAllAsync(
        this ConfiguredValueTaskAwaitable<Result> task,
        CancellationToken cancellationToken,
        params Func<ConfiguredValueTaskAwaitable<Result>>[] funcs
    )
    {
        return IfSuccessAllCore(task, cancellationToken, funcs).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfSuccessAllCore(
        this ConfiguredValueTaskAwaitable<Result> task,
        CancellationToken cancellationToken,
        params Func<ConfiguredValueTaskAwaitable<Result>>[] funcs
    )
    {
        var result = await task;

        if (result.IsHasError)
        {
            return new Result(result.Errors);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }

        var tasks = funcs.Select(x => x.Invoke()).ToArray();
        var errors = ReadOnlyMemory<Error>.Empty;

        foreach (var awaitable in tasks)
        {
            var r = await awaitable;

            if (cancellationToken.IsCancellationRequested)
            {
                return Result.CanceledByUserError;
            }

            errors = errors.Combine(r.Errors);
        }

        if (errors.IsEmpty)
        {
            return Result.Success;
        }

        return new Result(errors);
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAllAsync<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        CancellationToken cancellationToke,
        params Func<TValue, ConfiguredValueTaskAwaitable<Result>>[] funcs
    )
    {
        return IfSuccessAllCore(task, cancellationToke, funcs).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfSuccessAllCore<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        CancellationToken cancellationToken,
        params Func<TValue, ConfiguredValueTaskAwaitable<Result>>[] funcs
    )
    {
        var result = await task;

        if (result.IsHasError)
        {
            return new Result(result.Errors);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }

        var tasks = funcs.Select(x => x.Invoke(result.Value)).ToArray();
        var errors = ReadOnlyMemory<Error>.Empty;

        foreach (var awaitable in tasks)
        {
            var r = await awaitable;

            if (cancellationToken.IsCancellationRequested)
            {
                return Result.CanceledByUserError;
            }

            errors = errors.Combine(r.Errors);
        }

        return new Result(errors);
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAsync<TValue>(
        this Result<TValue> result,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> action,
        CancellationToken cancellationToken
    )
    {
        return IfSuccessCore(result, action, cancellationToken).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfSuccessCore<TValue>(
        this Result<TValue> result,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> action,
        CancellationToken cancellationToken
    )
    {
        if (result.IsHasError)
        {
            return new Result(result.Errors);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }

        return await action.Invoke(result.Value);
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAsync<TValue, TArg>(
        this Result<TValue> result,
        Result<TArg> arg,
        Func<TValue, TArg, ConfiguredValueTaskAwaitable<Result>> action,
        CancellationToken cancellationToken
    )
    {
        var errors = result.Errors.Combine(arg.Errors);

        if (!errors.IsEmpty)
        {
            return new Result(errors).ToValueTaskResult().ConfigureAwait(false);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result.CanceledByUserError.ToValueTaskResult().ConfigureAwait(false);
        }

        return action.Invoke(result.Value, arg.Value);
    }

    public static ConfiguredValueTaskAwaitable<Result<TReturn>> IfSuccessAsync<TValue, TReturn>(
        this Result<TValue> result,
        Func<TValue, ConfiguredValueTaskAwaitable<Result<TReturn>>> action,
        CancellationToken cancellationToken
    )
    {
        if (result.IsHasError)
        {
            return new Result<TReturn>(result.Errors).ToValueTaskResult().ConfigureAwait(false);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result<TReturn>.CanceledByUserError.ToValueTaskResult().ConfigureAwait(false);
        }

        return action.Invoke(result.Value);
    }

    public static ConfiguredValueTaskAwaitable<Result<TReturn>> IfSuccessAsync<TValue, TReturn>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result<TReturn>>> action,
        CancellationToken cancellationToken
    )
    {
        return IfSuccessCore(task, action, cancellationToken).ConfigureAwait(false);
    }

    private static async ValueTask<Result<TReturn>> IfSuccessCore<TValue, TReturn>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result<TReturn>>> action,
        CancellationToken cancellationToken
    )
    {
        var result = await task;

        if (result.IsHasError)
        {
            return new Result<TReturn>(result.Errors);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result<TReturn>.CanceledByUserError;
        }

        return await action.Invoke(result.Value);
    }

    public static ConfiguredValueTaskAwaitable<Result<TReturn>> IfSuccessAsync<TValue, TReturn>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, Result<TReturn>> func,
        CancellationToken cancellationToken
    )
    {
        return IfSuccessCore(task, func, cancellationToken).ConfigureAwait(false);
    }

    private static async ValueTask<Result<TReturn>> IfSuccessCore<TValue, TReturn>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, Result<TReturn>> func,
        CancellationToken cancellationToken
    )
    {
        var result = await task;

        if (result.IsHasError)
        {
            return new Result<TReturn>(result.Errors);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result<TReturn>.CanceledByUserError;
        }

        return func.Invoke(result.Value);
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAsync<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> action,
        CancellationToken cancellationToken
    )
    {
        return IfSuccessCore(task, action, cancellationToken).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfSuccessCore<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> action,
        CancellationToken cancellationToken
    )
    {
        var result = await task;

        if (result.IsHasError)
        {
            return new Result(result.Errors);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }

        return await action.Invoke(result.Value);
    }

    public static Result<TReturn> IfSuccess<TValue, TReturn>(
        this Result<TValue> result,
        Func<TValue, Result<TReturn>> action
    )
    {
        if (result.IsHasError)
        {
            return new Result<TReturn>(result.Errors);
        }

        return action.Invoke(result.Value);
    }

    public static ConfiguredValueTaskAwaitable<Result> IfSuccessTryFinallyAsync(
        this ConfiguredValueTaskAwaitable<Result> task,
        Func<ConfiguredValueTaskAwaitable<Result>> funcTry,
        Func<ConfiguredValueTaskAwaitable> funcFinally,
        CancellationToken cancellationToken
    )
    {
        return IfSuccessTryFinallyCore(task, funcTry, funcFinally, cancellationToken).ConfigureAwait(false);
    }

    private static async ValueTask<Result> IfSuccessTryFinallyCore(
        this ConfiguredValueTaskAwaitable<Result> task,
        Func<ConfiguredValueTaskAwaitable<Result>> funcTry,
        Func<ConfiguredValueTaskAwaitable> funcFinally,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var result = await task;

            if (result.IsHasError)
            {
                return new Result(result.Errors);
            }

            if (cancellationToken.IsCancellationRequested)
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