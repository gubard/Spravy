using Spravy.Domain.Models;

namespace Spravy.Domain.Extensions;

public static class ResultExtension
{
    public static Result<TReturn> IfSuccess<TValue, TReturn>(
        this Result<TValue> result,
        Func<TValue, Result<TReturn>> action
    )
    {
        if (result.IsHasError)
        {
            return new Result<TReturn>(result.Errors);
        }

        return action.Invoke(result.Value.ThrowIfNull());
    }

    public static Task<Result> IfSuccessAsync<TValue>(this Result<TValue> result, Func<TValue, Task<Result>> action)
    {
        if (result.IsHasError)
        {
            return new Result(result.Errors).ToTaskResult();
        }

        return action.Invoke(result.Value.ThrowIfNull());
    }

    public static Task<Result<TReturn>> IfSuccessAsync<TValue, TReturn>(
        this Result<TValue> result,
        Func<TValue, Task<Result<TReturn>>> action
    )
    {
        if (result.IsHasError)
        {
            return new Result<TReturn>(result.Errors).ToTaskResult();
        }

        return action.Invoke(result.Value.ThrowIfNull());
    }

    public static async Task<Result> IfSuccessAsync<TValue>(this Task<Result<TValue>> task, Action<TValue> action)
    {
        var result = await task;

        if (result.IsHasError)
        {
            return new Result(result.Errors);
        }

        action.Invoke(result.Value.ThrowIfNull());

        return Result.Success;
    }

    public static async Task<Result> IfSuccessAsync<TValue>(
        this Task<Result<TValue>> task,
        Func<TValue, Result> action
    )
    {
        var result = await task;

        if (result.IsHasError)
        {
            return new Result(result.Errors);
        }

        return action.Invoke(result.Value.ThrowIfNull());
    }

    public static async Task<Result> IfSuccessAsync<TValue>(
        this Task<Result<TValue>> task,
        Func<TValue, Task<Result>> action
    )
    {
        var result = await task;

        if (result.IsHasError)
        {
            return new Result(result.Errors);
        }

        return await action.Invoke(result.Value.ThrowIfNull());
    }

    public static async Task<Result> IfSuccessAsync<TValue, TArg1, TArg2>(
        this Task<Result<TValue>> task,
        Result<TArg1> arg1,
        Result<TArg2> arg2,
        Func<TValue, TArg1, TArg2, Task<Result>> func
    )
    {
        var result = await task;

        if (arg1.IsHasError)
        {
            if (arg2.IsHasError)
            {
                return new Result(arg1.Errors.Combine(arg2.Errors));
            }
            else
            {
                return new Result(arg1.Errors);
            }
        }

        if (arg2.IsHasError)
        {
            return new Result(arg2.Errors);
        }

        if (result.IsHasError)
        {
            return new Result(result.Errors);
        }

        return await func.Invoke(result.Value.ThrowIfNull(), arg1.Value, arg2.Value);
    }

    public static async Task<Result> IfSuccessAsync<TValue, TArg>(
        this Task<Result<TValue>> task,
        Result<TArg> arg,
        Func<TValue, TArg, Task<Result>> func
    )
    {
        var result = await task;

        if (arg.IsHasError)
        {
            return new Result(arg.Errors);
        }

        if (result.IsHasError)
        {
            return new Result(result.Errors);
        }

        return await func.Invoke(result.Value.ThrowIfNull(), arg.Value);
    }

    public static async Task<Result<TReturn>> IfSuccessAsync<TValue, TArg, TReturn>(
        this Task<Result<TValue>> task,
        Result<TArg> arg,
        Func<TValue, TArg, Task<Result<TReturn>>> func
    )
    {
        var result = await task;

        if (arg.IsHasError)
        {
            return new Result<TReturn>(arg.Errors);
        }

        if (result.IsHasError)
        {
            return new Result<TReturn>(result.Errors);
        }

        return await func.Invoke(result.Value.ThrowIfNull(), arg.Value);
    }

    public static async Task<Result<TReturn>> IfSuccessAsync<TValue, TReturn>(
        this Task<Result<TValue>> task,
        Func<TValue, TReturn> action
    )
    {
        var result = await task;

        if (result.IsHasError)
        {
            return new Result<TReturn>(result.Errors);
        }

        return new Result<TReturn>(action.Invoke(result.Value.ThrowIfNull()));
    }

    public static async Task<Result<TReturn>> IfSuccessAsync<TValue, TReturn>(
        this Task<Result<TValue>> task,
        Func<TValue, Task<Result<TReturn>>> action
    )
    {
        var result = await task;

        if (result.IsHasError)
        {
            return new Result<TReturn>(result.Errors);
        }

        return await action.Invoke(result.Value.ThrowIfNull());
    }
}