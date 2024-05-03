using System.Runtime.CompilerServices;
using Spravy.Domain.Errors;
using Spravy.Domain.Exceptions;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;

namespace Spravy.Domain.Models;

public class Result
{
    public static readonly Result CanceledByUserError = new(new CanceledByUserError());
    public static readonly Result Success = new(true);
    public static readonly ValueTask<Result> SuccessValueTask = ValueTask.FromResult(Success);
    public static readonly ConfiguredValueTaskAwaitable<Result> AwaitableFalse = SuccessValueTask.ConfigureAwait(false);

    private Result(bool _)
    {
    }

    public Result()
    {
        Errors = new([DefaultObject<DefaultCtorResultError>.Default,]);
    }

    public Result(ReadOnlyMemory<Error> errors)
    {
        Errors = errors;
    }

    public Result(Error error)
    {
        Errors = new([error,]);
    }

    public ReadOnlyMemory<Error> Errors { get; }

    public bool IsHasError
    {
        get => !Errors.IsEmpty;
    }
    
    public static Result Execute(Action action)
    {
        action.Invoke();
        
        return Result.Success;
    }
}

public class Result<TValue>
{
    public static readonly Result<TValue> CanceledByUserError = new(new CanceledByUserError());
    public static readonly Result<TValue> DefaultSuccess = new(default(TValue));
    public static readonly ValueTask<Result<TValue>> DefaultSuccessValueTask = ValueTask.FromResult(DefaultSuccess);

    public static readonly ConfiguredValueTaskAwaitable<Result<TValue>> DefaultAwaitableFalse =
        DefaultSuccessValueTask.ConfigureAwait(false);

    private readonly TValue value;

    public Result()
    {
        Errors = new([DefaultObject<DefaultCtorResultError<TValue>>.Default,]);
    }

    public Result(ReadOnlyMemory<Error> errors)
    {
        Errors = errors;

        if (Errors.IsEmpty)
        {
            throw new EmptyEnumerableException(nameof(Errors));
        }
    }

    public Result(Error error)
    {
        Errors = new([error,]);
    }

    public Result(TValue value)
    {
        this.value = value;
    }

    public ReadOnlyMemory<Error> Errors { get; }

    public TValue Value
    {
        get => IsHasError ? throw new ErrorsException(Errors) : value;
    }

    public bool IsHasError
    {
        get => !Errors.IsEmpty;
    }
}