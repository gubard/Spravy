using Spravy.Domain.Models;

namespace Spravy.Domain.Extensions;

public static class ResultExtension
{
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