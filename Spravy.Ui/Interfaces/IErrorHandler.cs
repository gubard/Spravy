namespace Spravy.Ui.Interfaces;

public interface IErrorHandler
{
    ConfiguredValueTaskAwaitable<Result> ErrorsHandleAsync(
        ReadOnlyMemory<Error> errors,
        CancellationToken token
    );
    ConfiguredValueTaskAwaitable<Result> ExceptionHandleAsync(
        Exception exception,
        CancellationToken token
    );
}
