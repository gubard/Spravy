namespace Spravy.Ui.Services;

public class ErrorHandler : IErrorHandler
{
    private static readonly ReadOnlyMemory<Guid> ignoreIds = new[] { CanceledByUserError.MainId, };

    private readonly IDialogViewer dialogViewer;
    private readonly IServiceFactory serviceFactory;

    public ErrorHandler(IDialogViewer dialogViewer, IServiceFactory serviceFactory)
    {
        this.dialogViewer = dialogViewer;
        this.serviceFactory = serviceFactory;
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

        errors = errors.Where(x => !ignoreIds.Span.Contains(x.Id));

        if (errors.IsEmpty)
        {
            return Result.AwaitableSuccess;
        }

        Console.WriteLine(errors.Select(x => x.Message).ToArray().JoinString(Environment.NewLine));
        var viewFactory = serviceFactory.CreateService<IViewFactory>();

        return dialogViewer.ShowInfoDialogAsync(
            viewFactory,
            DialogViewLayer.Error,
            viewFactory.CreateErrorViewModel(errors),
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

        Console.WriteLine(exception);
        var viewFactory = serviceFactory.CreateService<IViewFactory>();

        return dialogViewer.ShowInfoDialogAsync(
            viewFactory,
            DialogViewLayer.Error,
            viewFactory.CreateExceptionViewModel(exception),
            ct
        );
    }
}
