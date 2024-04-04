using System.Runtime.CompilerServices;
using Spravy.Domain.Models;

namespace Spravy.Domain.Extensions;

public static class ResultExtension
{
    public static async ValueTask<Result> ToValueTaskResultOnly(this Task task)
    {
        await task;

        return Result.Success;
    }

    public static async ValueTask<Result> ToResultOnly<TValue>(this ConfiguredValueTaskAwaitable<Result<TValue>> task)
    {
        var result = await task;

        if (result.IsHasError)
        {
            return new Result(result.Errors);
        }

        return Result.Success;
    }

    public static async ValueTask<Result<TReturn>> IfSuccessAsync<TReturn>(
        this ConfiguredValueTaskAwaitable<Result> task,
        Func<ConfiguredValueTaskAwaitable<Result<TReturn>>> func
    )
    {
        var result = await task;

        if (result.IsHasError)
        {
            return new Result<TReturn>(result.Errors);
        }

        return await func.Invoke();
    }

    public static async ValueTask<Result> IfSuccessAsync<TValue, TArg>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Result<TArg> arg,
        Func<TValue, TArg, ConfiguredValueTaskAwaitable<Result>> func
    )
    {
        if (arg.IsHasError)
        {
            return new Result(arg.Errors);
        }

        var result = await task;

        if (result.IsHasError)
        {
            return new Result(result.Errors);
        }

        return await func.Invoke(result.Value, arg.Value);
    }

    public static async ValueTask<Result<TReturn>> IfSuccessAsync<TValue, TArg, TReturn>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Result<TArg> arg,
        Func<TValue, TArg, ConfiguredValueTaskAwaitable<Result<TReturn>>> func
    )
    {
        if (arg.IsHasError)
        {
            return new Result<TReturn>(arg.Errors);
        }

        var result = await task;

        if (result.IsHasError)
        {
            return new Result<TReturn>(result.Errors);
        }

        return await func.Invoke(result.Value, arg.Value);
    }

    public static async ValueTask<Result<TReturn>> IfSuccessAsync<TArg, TReturn>(
        this ConfiguredValueTaskAwaitable<Result> task,
        Result<TArg> arg,
        Func<TArg, ConfiguredValueTaskAwaitable<Result<TReturn>>> func
    )
    {
        if (arg.IsHasError)
        {
            return new Result<TReturn>(arg.Errors);
        }

        var result = await task;

        if (result.IsHasError)
        {
            return new Result<TReturn>(result.Errors);
        }

        return await func.Invoke(arg.Value);
    }

    public static async ValueTask<Result<TReturn>> IfSuccessAsync<TArg1, TArg2, TReturn>(
        this ConfiguredValueTaskAwaitable<Result> task,
        Result<TArg1> arg1,
        Result<TArg2> arg2,
        Func<TArg1, TArg2, ConfiguredValueTaskAwaitable<Result<TReturn>>> func
    )
    {
        var arg = new Result(arg1.Errors.Combine(arg2.Errors));

        if (arg.IsHasError)
        {
            return new Result<TReturn>(arg.Errors);
        }

        var result = await task;

        if (result.IsHasError)
        {
            return new Result<TReturn>(result.Errors);
        }

        return await func.Invoke(arg1.Value, arg2.Value);
    }

    public static async ValueTask<Result<TReturn>> IfSuccessAsync<TValue, TArg1, TArg2, TReturn>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Result<TArg1> arg1,
        Result<TArg2> arg2,
        Func<TValue, TArg1, TArg2, ConfiguredValueTaskAwaitable<Result<TReturn>>> func
    )
    {
        var arg = new Result(arg1.Errors.Combine(arg2.Errors));

        if (arg.IsHasError)
        {
            return new Result<TReturn>(arg.Errors);
        }

        var result = await task;

        if (result.IsHasError)
        {
            return new Result<TReturn>(result.Errors);
        }

        return await func.Invoke(result.Value, arg1.Value, arg2.Value);
    }

    public static async ValueTask<Result> IfSuccessAsync<TValue, TArg1, TArg2>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Result<TArg1> arg1,
        Result<TArg2> arg2,
        Func<TValue, TArg1, TArg2, ConfiguredValueTaskAwaitable<Result>> func
    )
    {
        var arg = new Result(arg1.Errors.Combine(arg2.Errors));

        if (arg.IsHasError)
        {
            return new Result(arg.Errors);
        }

        var result = await task;

        if (result.IsHasError)
        {
            return new Result(result.Errors);
        }

        return await func.Invoke(result.Value, arg1.Value, arg2.Value);
    }

    public static async ValueTask<Result> IfSuccessAsync(
        this ConfiguredValueTaskAwaitable<Result> task,
        Func<ConfiguredValueTaskAwaitable<Result>> func
    )
    {
        var result = await task;

        if (result.IsHasError)
        {
            return new Result(result.Errors);
        }

        return await func.Invoke();
    }

    public static async ValueTask<Result> IfSuccessAllAsync(
        this ConfiguredValueTaskAwaitable<Result> task,
        params Func<ConfiguredValueTaskAwaitable<Result>>[] funcs
    )
    {
        var tasks = funcs.Select(x => x.Invoke()).ToArray();
        var result = await task;

        foreach (var awaitable in tasks)
        {
            var r = await awaitable;
            result = new Result(result.Errors.Combine(r.Errors));
        }

        return result;
    }

    public static async ValueTask<Result> IfSuccessAllAsync<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        params Func<TValue, ConfiguredValueTaskAwaitable<Result>>[] funcs
    )
    {
        var result = await task;

        if (result.IsHasError)
        {
            return new Result(result.Errors);
        }

        var tasks = funcs.Select(x => x.Invoke(result.Value)).ToArray();
        var res = Result.Success;

        foreach (var awaitable in tasks)
        {
            var r = await awaitable;
            res = new Result(res.Errors.Combine(r.Errors));
        }

        return res;
    }

    public static async ValueTask<Result> IfSuccessAsync<TValue>(
        this Result<TValue> result,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> action
    )
    {
        if (result.IsHasError)
        {
            return new Result(result.Errors);
        }

        return await action.Invoke(result.Value);
    }

    public static async ValueTask<Result<TReturn>> IfSuccessAsync<TValue, TReturn>(
        this Result<TValue> result,
        Func<TValue, ConfiguredValueTaskAwaitable<Result<TReturn>>> action
    )
    {
        if (result.IsHasError)
        {
            return new Result<TReturn>(result.Errors);
        }

        return await action.Invoke(result.Value);
    }

    public static async ValueTask<Result<TReturn>> IfSuccessAsync<TValue, TReturn>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result<TReturn>>> action
    )
    {
        var result = await task;

        if (result.IsHasError)
        {
            return new Result<TReturn>(result.Errors);
        }

        return await action.Invoke(result.Value);
    }

    public static async ValueTask<Result> IfSuccessAsync<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> action
    )
    {
        var result = await task;

        if (result.IsHasError)
        {
            return new Result(result.Errors);
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

    public static async ValueTask<Result> IfSuccessTryFinallyAsync(
        this ConfiguredValueTaskAwaitable<Result> task,
        Func<ConfiguredValueTaskAwaitable<Result>> funcTry,
        Func<ConfiguredValueTaskAwaitable> funcFinally
    )
    {
        try
        {
            var result = await task;

            if (result.IsHasError)
            {
                return new Result(result.Errors);
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