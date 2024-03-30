using Spravy.Domain.Exceptions;
using Spravy.Domain.Helpers;
using Spravy.Domain.ValidationResults;

namespace Spravy.Domain.Models;

public readonly struct Result
{
    public static readonly Result Success = new(true);

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

public readonly struct Result<TValue>
{
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
        Value = value;
    }

    public ReadOnlyMemory<Error> Errors { get; }
    public TValue? Value { get; }

    public bool IsHasError => !Errors.IsEmpty;
}