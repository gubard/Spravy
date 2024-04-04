using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Spravy.Domain.Models;
using Spravy.Ui.Models;

namespace Spravy.Ui.Interfaces;

public interface IDialogViewer
{
    ValueTask<Result> ShowContentDialogAsync<TView>(Action<TView> setupView, CancellationToken cancellationToken)
        where TView : ViewModelBase;

    ValueTask<Result> ShowProgressDialogAsync<TView>(Action<TView> setupView, CancellationToken cancellationToken)
        where TView : ViewModelBase;

    ValueTask<Result> ShowErrorDialogAsync<TView>(Action<TView> setupView, CancellationToken cancellationToken)
        where TView : ViewModelBase;

    ValueTask<Result> ShowInfoErrorDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> okTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    )
        where TView : ViewModelBase;

    ValueTask<Result> ShowInfoInputDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> okTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    )
        where TView : ViewModelBase;

    ValueTask<Result> ShowInfoContentDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> okTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    )
        where TView : ViewModelBase;

    ValueTask<Result> ShowInputDialogAsync<TView>(Action<TView> setupView, CancellationToken cancellationToken)
        where TView : ViewModelBase;

    ValueTask<Result> CloseContentDialogAsync(CancellationToken cancellationToken);
    ValueTask<Result> CloseErrorDialogAsync(CancellationToken cancellationToken);
    ValueTask<Result> CloseInputDialogAsync(CancellationToken cancellationToken);
    ValueTask<Result> CloseProgressDialogAsync(CancellationToken cancellationToken);
    ValueTask<Result<bool>> CloseLastDialogAsync(CancellationToken cancellationToken);

    ValueTask<Result> ShowConfirmContentDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Func<TView, ConfiguredValueTaskAwaitable<Result>> cancelTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase;

    ValueTask<Result> ShowConfirmInputDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Func<TView, ConfiguredValueTaskAwaitable<Result>> cancelTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase;
}