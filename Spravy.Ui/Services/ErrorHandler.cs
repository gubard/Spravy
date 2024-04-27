using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Serilog;
using Spravy.Client.Exceptions;
using Spravy.Domain.Errors;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.Ui.Features.ErrorHandling.ViewModels;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Services;

public class ErrorHandler : IErrorHandler
{
    private static readonly ReadOnlyMemory<Guid> IgnoreIds = new[]
    {
        CanceledByUserError.MainId,
    };

    private readonly IDialogViewer dialogViewer;

    public ErrorHandler(IDialogViewer dialogViewer)
    {
        this.dialogViewer = dialogViewer;
    }

    public ConfiguredValueTaskAwaitable<Result> ErrorsHandleAsync(ReadOnlyMemory<Error> errors, CancellationToken token)
    {
        if (errors.IsEmpty)
        {
            return Result.AwaitableFalse;
        }

        errors = errors.ToArray().Where(x => !IgnoreIds.Span.Contains(x.Id)).ToArray();

        if (errors.IsEmpty)
        {
            return Result.AwaitableFalse;
        }

        return dialogViewer.ShowInfoErrorDialogAsync<ErrorViewModel>(
            _ => dialogViewer.CloseErrorDialogAsync(CancellationToken.None)
                .IfSuccessAsync(
                    () => dialogViewer.CloseProgressDialogAsync(CancellationToken.None),
                    CancellationToken.None
                ),
            viewModel => viewModel.Errors.AddRange(errors.ToArray()),
            CancellationToken.None
        );
    }

    public ConfiguredValueTaskAwaitable<Result> ExceptionHandleAsync(Exception exception, CancellationToken token)
    {
        if (exception is TaskCanceledException)
        {
            return Result.AwaitableFalse;
        }

        if (exception is RpcException rpc)
        {
            switch (rpc.StatusCode)
            {
                case StatusCode.Cancelled:
                    return Result.AwaitableFalse;
            }
        }

        if (exception is GrpcException { InnerException: RpcException rpc2 })
        {
            switch (rpc2.StatusCode)
            {
                case StatusCode.Cancelled:
                    return Result.AwaitableFalse;
            }
        }

        Log.Logger.Error(exception, "UI error");

        return dialogViewer.ShowInfoErrorDialogAsync<ExceptionViewModel>(
            _ => dialogViewer.CloseErrorDialogAsync(token)
                .IfSuccessAsync(
                    () => dialogViewer.CloseProgressDialogAsync(token),
                    token
                ),
            viewModel => viewModel.Exception = exception,
            token
        );
    }
}