using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Spravy.Domain.Models;
using Spravy.Ui.Interfaces;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Extensions;

public static class ConfiguredTaskAwaitableExtension
{
    public static async Task IfSuccessAsync(
        this ConfiguredTaskAwaitable<Error> task,
        IDialogViewer dialogViewer,
        Func<Task> func
    )
    {
        var error = await task;

        if (error.IsError)
        {
            await dialogViewer.ShowInfoErrorDialogAsync<ExceptionViewModel>(
                async _ =>
                {
                    await dialogViewer.CloseErrorDialogAsync(CancellationToken.None);
                    await dialogViewer.CloseProgressDialogAsync(CancellationToken.None);
                },
                viewModel => viewModel.Exception = new Exception(),
                CancellationToken.None
            );
        }
        else
        {
            await func.Invoke().ConfigureAwait(false);
        }
    }
}