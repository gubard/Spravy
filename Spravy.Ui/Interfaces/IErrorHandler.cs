namespace Spravy.Ui.Interfaces;

public interface IErrorHandler
{
    ConfiguredValueTaskAwaitable<Result> ErrorsHandleAsync(
        ReadOnlyMemory<Error> errors,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result> ExceptionHandleAsync(
        Exception exception,
        CancellationToken ct
    );
}
