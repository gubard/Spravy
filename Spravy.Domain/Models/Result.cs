namespace Spravy.Domain.Models;

public class Result
{
    public static readonly Result CanceledByUserError = new(new CanceledByUserError());
    public static readonly ValueTask<Result> CanceledByUserErrorValueTask = ValueTask.FromResult(
        CanceledByUserError
    );

    public static readonly ConfiguredValueTaskAwaitable<Result> AwaitableCanceledByUserError =
        CanceledByUserErrorValueTask.ConfigureAwait(false);

    public static readonly Result Success = new(true);
    public static readonly ValueTask<Result> SuccessValueTask = ValueTask.FromResult(Success);

    public static readonly ConfiguredValueTaskAwaitable<Result> AwaitableSuccess =
        SuccessValueTask.ConfigureAwait(false);

    private Result(bool _) { }

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

        return Success;
    }
}

public class Result<TValue>
    where TValue : notnull
{
    public static readonly Result<TValue> CanceledByUserError = new(new CanceledByUserError());

    private readonly TValue? value;

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
        get => IsHasError || value is null ? throw new ErrorsException(Errors) : value;
    }

    public bool IsHasError
    {
        get => !Errors.IsEmpty;
    }
}
