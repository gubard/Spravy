using System.Runtime.CompilerServices;
using Spravy.Domain.Errors;
using Spravy.Domain.Exceptions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Models;

public class Result
{
    public static readonly Result Success = new(true);
    public static readonly ValueTask<Result> SuccessValueTask = ValueTask.FromResult(Success);
    public static readonly ConfiguredValueTaskAwaitable<Result> AwaitableFalse = SuccessValueTask.ConfigureAwait(false);

    private Result(bool _)
    {
    }

    public Result()
    {
        Errors = new ReadOnlyMemory<Error>([DefaultObject<DefaultCtorResultError>.Default]);
    }

    public Result(ReadOnlyMemory<Error> errors)
    {
        Errors = errors;
    }

    public Result(Error error)
    {
        Errors = new ReadOnlyMemory<Error>([error]);
    }

    public ReadOnlyMemory<Error> Errors { get; }

    public bool IsHasError => !Errors.IsEmpty;
}

public class Result<TValue>
{
    public static readonly Result<TValue> DefaultSuccess = new(default(TValue));
    public static readonly ValueTask<Result<TValue>> DefaultSuccessValueTask = ValueTask.FromResult(DefaultSuccess);

    public static readonly ConfiguredValueTaskAwaitable<Result<TValue>> DefaultAwaitableFalse =
        DefaultSuccessValueTask.ConfigureAwait(false);

    private readonly TValue value = default;

    public Result()
    {
        Errors = new ReadOnlyMemory<Error>([DefaultObject<DefaultCtorResultError<TValue>>.Default]);
    }

    public Result(ReadOnlyMemory<Error> errors)
    {
        Errors = errors;

        if (Errors.IsEmpty)
        {
            throw new EmptyEnumerableException(nameof(Errors));
        }
    }

    public Result(Error validationResults)
    {
        Errors = new ReadOnlyMemory<Error>([validationResults]);
    }

    public Result(TValue value)
    {
        this.value = value;
    }

    public ReadOnlyMemory<Error> Errors { get; }

    public TValue Value => IsHasError ? throw new Exception("Result has error") : value;

    public bool IsHasError => !Errors.IsEmpty;
}