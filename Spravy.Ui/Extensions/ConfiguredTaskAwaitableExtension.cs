using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.Ui.Features.ErrorHandling.ViewModels;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Extensions;

public static class ConfiguredTaskAwaitableExtension
{
    public static async Task IfSuccessAsync(
        this ConfiguredTaskAwaitable<Result> task,
        IDialogViewer dialogViewer,
        Func<Task> func
    )
    {
        var error = await task;

        if (error.IsHasError)
        {
            await dialogViewer.ShowInfoErrorDialogAsync<ErrorViewModel>(
                async _ =>
                {
                    await dialogViewer.CloseErrorDialogAsync(CancellationToken.None);
                    await dialogViewer.CloseProgressDialogAsync(CancellationToken.None);
                },
                viewModel => viewModel.ValidationResults.AddRange(error.Errors.ToArray()),
                CancellationToken.None
            );
        }
        else
        {
            await func.Invoke().ConfigureAwait(false);
        }
    }

    public static async Task IfSuccessAsync<TValue>(
        this ConfiguredTaskAwaitable<Result<TValue>> task,
        IDialogViewer dialogViewer,
        Func<TValue, Task> func
    )
    {
        var error = await task;

        if (error.IsHasError)
        {
            await dialogViewer.ShowInfoErrorDialogAsync<ErrorViewModel>(
                async _ =>
                {
                    await dialogViewer.CloseErrorDialogAsync(CancellationToken.None);
                    await dialogViewer.CloseProgressDialogAsync(CancellationToken.None);
                },
                viewModel => viewModel.ValidationResults.AddRange(error.Errors.ToArray()),
                CancellationToken.None
            );
        }
        else
        {
            await func.Invoke(error.Value.ThrowIfNull()).ConfigureAwait(false);
        }
    }
}