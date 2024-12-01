namespace Spravy.Ui.Interfaces;

public interface IErrorHandler
{
    Cvtar ErrorsHandleAsync(ReadOnlyMemory<Error> errors, CancellationToken ct);

    Cvtar ExceptionHandleAsync(Exception exception, CancellationToken ct);
}