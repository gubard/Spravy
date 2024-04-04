using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Ninject;
using Spravy.Domain.Di.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.Ui.Features.ErrorHandling.ViewModels;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Services;

public class TaskWork
{
    private readonly Delegate del;
    private readonly IDialogViewer dialogViewer;
    private CancellationTokenSource cancellationTokenSource = new();
    private ValueTask<Result>? current;

    private TaskWork(Delegate del, IDialogViewer dialogViewer)
    {
        this.del = del;
        this.dialogViewer = dialogViewer;
    }

    public ValueTask<Result> Current => current.ThrowIfNullStruct();

    public async Task RunAsync()
    {
        Cancel();
        current = (ValueTask<Result>)del.DynamicInvoke(cancellationTokenSource.Token).ThrowIfNull();
        var result = await Current;

        if (result.IsHasError)
        {
            await dialogViewer.ShowInfoErrorDialogAsync<ErrorViewModel>(
                _ => dialogViewer.CloseErrorDialogAsync(CancellationToken.None)
                    .ConfigureAwait(false)
                    .IfSuccessAsync(
                        () => dialogViewer.CloseProgressDialogAsync(CancellationToken.None).ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
                viewModel => viewModel.ValidationResults.AddRange(result.Errors.ToArray()),
                CancellationToken.None
            );
        }
    }

    public async Task RunAsync<T>(T value)
    {
        Cancel();
        current = (ValueTask<Result>)del.DynamicInvoke(value, cancellationTokenSource.Token).ThrowIfNull();
        var result = await Current;

        if (result.IsHasError)
        {
            await dialogViewer.ShowInfoErrorDialogAsync<ErrorViewModel>(
                _ => dialogViewer.CloseErrorDialogAsync(CancellationToken.None)
                    .ConfigureAwait(false)
                    .IfSuccessAsync(
                        () => dialogViewer.CloseProgressDialogAsync(CancellationToken.None).ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
                viewModel => viewModel.ValidationResults.AddRange(result.Errors.ToArray()),
                CancellationToken.None
            );
        }
    }

    public void Cancel()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource = new();
    }

    public static TaskWork Create(Func<CancellationToken, ValueTask<Result>> task)
    {
        return new(task, DiHelper.Kernel.ThrowIfNull().Get<IDialogViewer>());
    }

    public static TaskWork Create<T>(Func<T, CancellationToken, ValueTask<Result>> task)
    {
        return new(task, DiHelper.Kernel.ThrowIfNull().Get<IDialogViewer>());
    }
}