namespace Spravy.Ui.Services;

public class ErrorHandler : IErrorHandler
{
    private static readonly ReadOnlyMemory<Guid> IgnoreIds = new[] { CanceledByUserError.MainId, };

    private readonly IDialogViewer dialogViewer;

    public ErrorHandler(IDialogViewer dialogViewer)
    {
        this.dialogViewer = dialogViewer;
    }

    public ConfiguredValueTaskAwaitable<Result> ErrorsHandleAsync(
        ReadOnlyMemory<Error> errors,
        CancellationToken ct
    )
    {
        if (errors.IsEmpty)
        {
            return Result.AwaitableSuccess;
        }

        errors = errors.Where(x => !IgnoreIds.Span.Contains(x.Id));

        if (errors.IsEmpty)
        {
            return Result.AwaitableSuccess;
        }

        return dialogViewer.ShowInfoErrorDialogAsync<ErrorViewModel>(
            _ =>
                dialogViewer
                    .CloseErrorDialogAsync(ct)
                    .IfSuccessAsync(() => dialogViewer.CloseProgressDialogAsync(ct), ct),
            viewModel => viewModel.Errors.AddRange(errors.ToArray()),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> ExceptionHandleAsync(
        Exception exception,
        CancellationToken ct
    )
    {
        if (exception is TaskCanceledException)
        {
            return Result.AwaitableSuccess;
        }

        if (exception is RpcException rpc)
        {
            switch (rpc.StatusCode)
            {
                case StatusCode.Cancelled:
                    return Result.AwaitableSuccess;
            }
        }

        if (exception is GrpcException { InnerException: RpcException rpc2, })
        {
            switch (rpc2.StatusCode)
            {
                case StatusCode.Cancelled:
                    return Result.AwaitableSuccess;
            }
        }

        Log.Logger.Error(exception, "UI error");

        return dialogViewer.ShowInfoErrorDialogAsync<ExceptionViewModel>(
            _ =>
                dialogViewer
                    .CloseErrorDialogAsync(ct)
                    .IfSuccessAsync(() => dialogViewer.CloseProgressDialogAsync(ct), ct),
            viewModel => viewModel.Exception = exception,
            ct
        );
    }
}
