using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Spravy.Domain.Models;
using Spravy.Ui.Models;

namespace Spravy.Ui.Interfaces;

public interface IDialogViewer
{
    ConfiguredValueTaskAwaitable<Result> ShowContentDialogAsync<TView>(Action<TView> setupView, CancellationToken cancellationToken)
        where TView : ViewModelBase;

    ConfiguredValueTaskAwaitable<Result> ShowProgressDialogAsync<TView>(Action<TView> setupView, CancellationToken cancellationToken)
        where TView : ViewModelBase;

    ConfiguredValueTaskAwaitable<Result> ShowErrorDialogAsync<TView>(Action<TView> setupView, CancellationToken cancellationToken)
        where TView : ViewModelBase;

    ConfiguredValueTaskAwaitable<Result> ShowInfoErrorDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> okTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    )
        where TView : ViewModelBase;

    ConfiguredValueTaskAwaitable<Result> ShowInfoInputDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> okTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    )
        where TView : ViewModelBase;

    ConfiguredValueTaskAwaitable<Result> ShowInfoContentDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> okTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    )
        where TView : ViewModelBase;

    ConfiguredValueTaskAwaitable<Result> ShowInputDialogAsync<TView>(Action<TView> setupView, CancellationToken cancellationToken)
        where TView : ViewModelBase;

    ConfiguredValueTaskAwaitable<Result> CloseContentDialogAsync(CancellationToken cancellationToken);
    ConfiguredValueTaskAwaitable<Result> CloseErrorDialogAsync(CancellationToken cancellationToken);
    ConfiguredValueTaskAwaitable<Result> CloseInputDialogAsync(CancellationToken cancellationToken);
    ConfiguredValueTaskAwaitable<Result> CloseProgressDialogAsync(CancellationToken cancellationToken);
    ConfiguredValueTaskAwaitable<Result<bool>> CloseLastDialogAsync(CancellationToken cancellationToken);

    ConfiguredValueTaskAwaitable<Result> ShowConfirmContentDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Func<TView, ConfiguredValueTaskAwaitable<Result>> cancelTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase;

    ConfiguredValueTaskAwaitable<Result> ShowConfirmInputDialogAsync<TView>(
        Func<TView, ConfiguredValueTaskAwaitable<Result>> confirmTask,
        Func<TView, ConfiguredValueTaskAwaitable<Result>> cancelTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase;
}