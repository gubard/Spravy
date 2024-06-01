namespace Spravy.Domain.Extensions;

public static class ResultExtension
{
    public static Result IfSuccessForEach<TValue>(this Result<ReadOnlyMemory<TValue>> values, Func<TValue, Result> func)
    {
        if (values.IsHasError)
        {
            return new(values.Errors);
        }
        
        var valuesArray = values.Value.ToArray();
        
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
    {
        if (values.IsHasError)
        {
            return new(values.Errors);
        }
        
        var array = new TReturn[values.Value.Length];
        var valuesArray = values.Value.ToArray();
        
        for (var index = 0; index < valuesArray.Length; index++)
        {
            var value = valuesArray[index];
            var result = func.Invoke(value);
            
            if (result.IsHasError)
            {
                return new(result.Errors);
            }
            
            array[index] = result.Value;
        }
        
        return array.ToReadOnlyMemory().ToResult();
    }
    
    public static ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TReturn>>> IfSuccessForEachAsync<TValue, TReturn>(
        this ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TValue>>> task,
        Func<TValue, Result<TReturn>> func,
        CancellationToken cancellationToken
    )
    {
        return IfSuccessForEachCore(task, func, cancellationToken).ConfigureAwait(false);
    }
    
    private static async ValueTask<Result<ReadOnlyMemory<TReturn>>> IfSuccessForEachCore<TValue, TReturn>(
        this ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TValue>>> task,
        Func<TValue, Result<TReturn>> func,
        CancellationToken cancellationToken
    )
    {
        var values = await task;
        
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<ReadOnlyMemory<TReturn>>.CanceledByUserError;
        }
        
        if (values.IsHasError)
        {
            return new(values.Errors);
        }
        
        var array = new TReturn[values.Value.Length];
        var valuesArray = values.Value.ToArray();
        
        for (var index = 0; index < valuesArray.Length; index++)
        {
            var value = valuesArray[index];
            var result = func.Invoke(value);
            
            if (result.IsHasError)
            {
                return new(result.Errors);
            }
            
            if (cancellationToken.IsCancellationRequested)
            {
                return Result<ReadOnlyMemory<TReturn>>.CanceledByUserError;
            }
            
            array[index] = result.Value;
        }
        
        return array.ToReadOnlyMemory().ToResult();
    }
    
    public static ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TReturn>>> IfSuccessForEachAsync<TValue, TReturn>(
        this Result<ReadOnlyMemory<TValue>> values,
        Func<TValue, ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        CancellationToken cancellationToken
    )
    {
        return IfSuccessForEachCore(values, func, cancellationToken).ConfigureAwait(false);
    }
    
    private static async ValueTask<Result<ReadOnlyMemory<TReturn>>> IfSuccessForEachCore<TValue, TReturn>(
        this Result<ReadOnlyMemory<TValue>> values,
        Func<TValue, ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        CancellationToken cancellationToken
    )
    {
        if (values.IsHasError)
        {
            return new(values.Errors);
        }
        
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<ReadOnlyMemory<TReturn>>.CanceledByUserError;
        }
        
        var array = new TReturn[values.Value.Length];
        var valuesArray = values.Value.ToArray();
        
        for (var index = 0; index < valuesArray.Length; index++)
        {
            var value = valuesArray[index];
            var result = await func.Invoke(value);
            
            if (result.IsHasError)
            {
                return new(result.Errors);
            }
            
            if (cancellationToken.IsCancellationRequested)
            {
                return Result<ReadOnlyMemory<TReturn>>.CanceledByUserError;
            }
            
            array[index] = result.Value;
        }
        
        return array.ToReadOnlyMemory().ToResult();
    }
    
    public static ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TReturn>>> IfSuccessForEachAsync<TValue, TReturn>(
        this ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TValue>>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        CancellationToken cancellationToken
    )
    {
        return IfSuccessForEachCore(task, func, cancellationToken).ConfigureAwait(false);
    }
    
    private static async ValueTask<Result<ReadOnlyMemory<TReturn>>> IfSuccessForEachCore<TValue, TReturn>(
        this ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TValue>>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        CancellationToken cancellationToken
    )
    {
        var values = await task;
        
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<ReadOnlyMemory<TReturn>>.CanceledByUserError;
        }
        
        if (values.IsHasError)
        {
            return new(values.Errors);
        }
        
        var array = new TReturn[values.Value.Length];
        var valuesArray = values.Value.ToArray();
        
        for (var index = 0; index < valuesArray.Length; index++)
        {
            var value = valuesArray[index];
            var result = await func.Invoke(value);
            
            if (result.IsHasError)
            {
                return new(result.Errors);
            }
            
            if (cancellationToken.IsCancellationRequested)
            {
                return Result<ReadOnlyMemory<TReturn>>.CanceledByUserError;
            }
            
            array[index] = result.Value;
        }
        
        return array.ToReadOnlyMemory().ToResult();
    }
    
    public static ConfiguredValueTaskAwaitable<Result> IfSuccessForEachAsync<TValue>(
        this Result<ReadOnlyMemory<TValue>> values,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken cancellationToken
    )
    {
        return IfSuccessForEachCore(values, func, cancellationToken).ConfigureAwait(false);
    }
    
    private static async ValueTask<Result> IfSuccessForEachCore<TValue>(
        this Result<ReadOnlyMemory<TValue>> values,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken cancellationToken
    )
    {
        if (values.IsHasError)
        {
            return new(values.Errors);
        }
        
        if (cancellationToken.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }
        
        foreach (var value in values.Value.ToArray())
        {
            var result = await func.Invoke(value);
            
            if (result.IsHasError)
            {
                return new(result.Errors);
            }
            
            if (cancellationToken.IsCancellationRequested)
            {
                return Result.CanceledByUserError;
            }
        }
        
        return Result.Success;
    }
    
    public static ConfiguredValueTaskAwaitable<Result> IfSuccessForEachAsync<TValue>(
        this ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TValue>>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken cancellationToken
    )
    {
        return IfSuccessForEachCore(task, func, cancellationToken).ConfigureAwait(false);
    }
    
    private static async ValueTask<Result> IfSuccessForEachCore<TValue>(
        this ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TValue>>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken cancellationToken
    )
    {
        var values = await task;
        
        if (values.IsHasError)
        {
            return new(values.Errors);
        }
        
        foreach (var value in values.Value.ToArray())
        {
            var result = await func.Invoke(value);
            
            if (result.IsHasError)
            {
                return new(result.Errors);
            }
            
            if (cancellationToken.IsCancellationRequested)
            {
                return Result.CanceledByUserError;
            }
        }
        
        return Result.Success;
    }
    
    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAsync<TValue>(
        this ConfiguredCancelableAsyncEnumerable<Result<TValue>> enumerable,
        Func<TValue, Result> func,
        CancellationToken cancellationToken
    )
    {
        return IfSuccessCore(enumerable, func, cancellationToken).ConfigureAwait(false);
    }
    
    private static async ValueTask<Result> IfSuccessCore<TValue>(
        this ConfiguredCancelableAsyncEnumerable<Result<TValue>> enumerable,
        Func<TValue, Result> func,
        CancellationToken cancellationToken
    )
    {
        await foreach (var result in enumerable)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Result.CanceledByUserError;
            }
            
            if (result.IsHasError)
            {
                return new(result.Errors);
            }
            
            var item = func.Invoke(result.Value);
            
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
        CancellationToken cancellationToken
    )
    {
        return IfSuccessAllInOrderCore(task, cancellationToken, funcs).ConfigureAwait(false);
    }
    
    private static async ValueTask<Result> IfSuccessAllInOrderCore<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        CancellationToken cancellationToken,
        Func<TValue, Func<ConfiguredValueTaskAwaitable<Result>>[]> funcs
    )
    {
        var result = await task;
        
        if (result.IsHasError)
        {
            return new(result.Errors);
        }
        
        if (cancellationToken.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }
        
        var awaitables = funcs.Invoke(result.Value);
        
        foreach (var awaitable in awaitables)
        {
            var r = await awaitable.Invoke();
            
            if (cancellationToken.IsCancellationRequested)
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
        CancellationToken cancellationToken
    )
    {
        if (cancellationToken.IsCancellationRequested)
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
        CancellationToken cancellationToken
    )
    {
        return IfErrorsCore(task, func, cancellationToken).ConfigureAwait(false);
    }
    
    private static async ValueTask<Result> IfErrorsCore(
        this ConfiguredValueTaskAwaitable<Result> task,
        Func<ReadOnlyMemory<Error>, Result> func,
        CancellationToken cancellationToken
    )
    {
        var result = await task;
        
        if (cancellationToken.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }
        
        if (result.IsHasError)
        {
            return func.Invoke(result.Errors);
        }
        
        return Result.Success;
    }
    
    public static ConfiguredValueTaskAwaitable<Result> ToResultOnlyAsync<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task
    )
    {
        return ToResultOnlyCore(task).ConfigureAwait(false);
    }
    
    private static async ValueTask<Result> ToResultOnlyCore<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task
    )
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
    {
        if (result.IsHasError)
        {
            throw new ErrorsException(result.Errors);
        }
        
        return result.Value;
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
            return new(result.Errors);
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
            return new(errors);
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
            return new(errors);
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
            return new(errors);
        }
        
        if (cancellationToken.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }
        
        return await func.Invoke(result.Value, arg1.Value, arg2.Value);
    }
    
    public static ConfiguredValueTaskAwaitable<Result<TReturn>> IfSuccessAsync<TReturn>(
        this ConfiguredValueTaskAwaitable<Result> task,
        Func<Result<TReturn>> func,
        CancellationToken cancellationToken
    )
    {
        return IfSuccessCore(task, func, cancellationToken).ConfigureAwait(false);
    }
    
    private static async ValueTask<Result<TReturn>> IfSuccessCore<TReturn>(
        this ConfiguredValueTaskAwaitable<Result> task,
        Func<Result<TReturn>> func,
        CancellationToken cancellationToken
    )
    {
        var result = await task;
        
        if (result.IsHasError)
        {
            return new(result.Errors);
        }
        
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<TReturn>.CanceledByUserError;
        }
        
        return func.Invoke();
    }
    
    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAsync(
        this ConfiguredValueTaskAwaitable<Result> task,
        Func<Result> func,
        CancellationToken cancellationToken
    )
    {
        return IfSuccessCore(task, func, cancellationToken).ConfigureAwait(false);
    }
    
    private static async ValueTask<Result> IfSuccessCore(
        this ConfiguredValueTaskAwaitable<Result> task,
        Func<Result> func,
        CancellationToken cancellationToken
    )
    {
        var result = await task;
        
        if (result.IsHasError)
        {
            return new(result.Errors);
        }
        
        if (cancellationToken.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }
        
        return func.Invoke();
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
            return new(result.Errors);
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
            return new(result.Errors);
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
        
        return new(errors);
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
            return new(result.Errors);
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
        
        return new(errors);
    }
    
    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAsync(
        this Result result,
        Func<ConfiguredValueTaskAwaitable<Result>> action,
        CancellationToken cancellationToken
    )
    {
        return IfSuccessCore(result, action, cancellationToken).ConfigureAwait(false);
    }
    
    private static async ValueTask<Result> IfSuccessCore(
        this Result result,
        Func<ConfiguredValueTaskAwaitable<Result>> action,
        CancellationToken cancellationToken
    )
    {
        if (result.IsHasError)
        {
            return new(result.Errors);
        }
        
        if (cancellationToken.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }
        
        return await action.Invoke();
    }
    
    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAsync<TValue>(
        this Result<TValue> result,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> action,
        Func<ReadOnlyMemory<Error>, ConfiguredValueTaskAwaitable<Result>> errors,
        CancellationToken cancellationToken
    )
    {
        if (result.IsHasError)
        {
            return errors.Invoke(result.Errors);
        }
        
        if (cancellationToken.IsCancellationRequested)
        {
            return Result.AwaitableCanceledByUserError;
        }
        
        return action.Invoke(result.Value);
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
            return new(result.Errors);
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
            return new(result.Errors);
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
            return new(result.Errors);
        }
        
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<TReturn>.CanceledByUserError;
        }
        
        return func.Invoke(result.Value);
    }
    
    public static ConfiguredValueTaskAwaitable<Result<TReturn>> IfSuccessDisposeAsync<TValue, TReturn>(
        this Result<TValue> result,
        Func<TValue, ConfiguredValueTaskAwaitable<Result<TReturn>>> action,
        CancellationToken cancellationToken
    ) where TValue : IAsyncDisposable
    {
        return IfSuccessDisposeCore(result, action, cancellationToken).ConfigureAwait(false);
    }
    
    private static async ValueTask<Result<TReturn>> IfSuccessDisposeCore<TValue, TReturn>(
        this Result<TValue> result,
        Func<TValue, ConfiguredValueTaskAwaitable<Result<TReturn>>> func,
        CancellationToken cancellationToken
    ) where TValue : IAsyncDisposable
    {
        if (result.IsHasError)
        {
            return new(result.Errors);
        }
        
        await using var value = result.Value;
        
        if (cancellationToken.IsCancellationRequested)
        {
            return Result<TReturn>.CanceledByUserError;
        }
        
        return await func.Invoke(value);
    }
    
    public static ConfiguredValueTaskAwaitable<Result> IfSuccessDisposeAsync<TValue>(
        this Result<TValue> result,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> action,
        CancellationToken cancellationToken
    ) where TValue : IAsyncDisposable
    {
        return IfSuccessDisposeCore(result, action, cancellationToken).ConfigureAwait(false);
    }
    
    private static async ValueTask<Result> IfSuccessDisposeCore<TValue>(
        this Result<TValue> result,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken cancellationToken
    ) where TValue : IAsyncDisposable
    {
        if (result.IsHasError)
        {
            return new(result.Errors);
        }
        
        await using var value = result.Value;
        
        if (cancellationToken.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }
        
        return await func.Invoke(value);
    }
    
    public static ConfiguredValueTaskAwaitable<Result> IfSuccessAsync<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, Result> action,
        CancellationToken cancellationToken
    )
    {
        return IfSuccessCore(task, action, cancellationToken).ConfigureAwait(false);
    }
    
    private static async ValueTask<Result> IfSuccessCore<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, Result> action,
        CancellationToken cancellationToken
    )
    {
        var result = await task;
        
        if (result.IsHasError)
        {
            return new(result.Errors);
        }
        
        if (cancellationToken.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }
        
        return action.Invoke(result.Value);
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
            return new(result.Errors);
        }
        
        if (cancellationToken.IsCancellationRequested)
        {
            return Result.CanceledByUserError;
        }
        
        return await action.Invoke(result.Value);
    }
    
    public static Result<TReturn> IfSuccess<TReturn>(this Result result, Func<Result<TReturn>> action)
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
    {
        if (result.IsHasError)
        {
            return new(result.Errors);
        }
        
        return action.Invoke(result.Value);
    }
    
    public static Result IfSuccess<TValue>(this Result<TValue> result, Func<TValue, Result> action)
    {
        if (result.IsHasError)
        {
            return new(result.Errors);
        }
        
        return action.Invoke(result.Value);
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
    {
        var errors = result.Errors.Combine(arg1.Errors, arg2.Errors);
        
        if (!errors.IsEmpty)
        {
            return new(errors);
        }
        
        return action.Invoke(arg1.Value, arg2.Value);
    }
    
    public static Result<TReturn> IfSuccess<TValue, TArg1, TArg2, TReturn>(
        this Result<TValue> result,
        Result<TArg1> arg1,
        Result<TArg2> arg2,
        Func<TValue, TArg1, TArg2, Result<TReturn>> action
    )
    {
        var errors = result.Errors.Combine(arg1.Errors, arg2.Errors);
        
        if (!errors.IsEmpty)
        {
            return new(errors);
        }
        
        return action.Invoke(result.Value, arg1.Value, arg2.Value);
    }
    
    public static ConfiguredValueTaskAwaitable<Result> IfSuccessTryFinallyAsync<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> funcTry,
        Action<TValue> funcFinally,
        CancellationToken cancellationToken
    )
    {
        return IfSuccessTryFinallyCore(task, funcTry, funcFinally, cancellationToken).ConfigureAwait(false);
    }
    
    private static async ValueTask<Result> IfSuccessTryFinallyCore<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> funcTry,
        Action<TValue> funcFinally,
        CancellationToken cancellationToken
    )
    {
        var result = await task;
        
        if (result.IsHasError)
        {
            return new(result.Errors);
        }
        
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Result.CanceledByUserError;
            }
            
            return await funcTry.Invoke(result.Value);
        }
        finally
        {
            funcFinally.Invoke(result.Value);
        }
    }
    
    public static ConfiguredValueTaskAwaitable<Result> IfSuccessTryFinallyAsync<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> funcTry,
        Func<TValue, ConfiguredValueTaskAwaitable> funcFinally,
        CancellationToken cancellationToken
    )
    {
        return IfSuccessTryFinallyCore(task, funcTry, funcFinally, cancellationToken).ConfigureAwait(false);
    }
    
    private static async ValueTask<Result> IfSuccessTryFinallyCore<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        Func<TValue, ConfiguredValueTaskAwaitable<Result>> funcTry,
        Func<TValue, ConfiguredValueTaskAwaitable> funcFinally,
        CancellationToken cancellationToken
    )
    {
        var result = await task;
        
        if (result.IsHasError)
        {
            return new(result.Errors);
        }
        
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Result.CanceledByUserError;
            }
            
            return await funcTry.Invoke(result.Value);
        }
        finally
        {
            await funcFinally.Invoke(result.Value);
        }
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
                return new(result.Errors);
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