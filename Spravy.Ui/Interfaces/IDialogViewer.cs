using System;
using System.Threading;
using System.Threading.Tasks;
using Spravy.Ui.Models;

namespace Spravy.Ui.Interfaces;

public interface IDialogViewer
{
    Task ShowContentDialogAsync<TView>(Action<TView> setupView, CancellationToken cancellationToken)
        where TView : ViewModelBase;

    Task ShowProgressDialogAsync<TView>(Action<TView> setupView, CancellationToken cancellationToken)
        where TView : ViewModelBase;

    Task ShowErrorDialogAsync<TView>(Action<TView> setupView, CancellationToken cancellationToken)
        where TView : ViewModelBase;

    Task ShowInfoErrorDialogAsync<TView>(
        Func<TView, Task> okTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    )
        where TView : ViewModelBase;

    Task ShowInfoInputDialogAsync<TView>(
        Func<TView, Task> okTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    )
        where TView : ViewModelBase;
    
    Task ShowInfoContentDialogAsync<TView>(
        Func<TView, Task> okTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    )
        where TView : ViewModelBase;

    Task ShowInputDialogAsync<TView>(Action<TView> setupView, CancellationToken cancellationToken)
        where TView : ViewModelBase;

    Task CloseContentDialogAsync(CancellationToken cancellationToken);
    Task CloseErrorDialogAsync(CancellationToken cancellationToken);
    Task CloseInputDialogAsync(CancellationToken cancellationToken);
    Task CloseProgressDialogAsync(CancellationToken cancellationToken);
    Task<bool> CloseLastDialogAsync(CancellationToken cancellationToken);

    Task ShowConfirmContentDialogAsync<TView>(
        Func<TView, Task> confirmTask,
        Func<TView, Task> cancelTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase;

    Task ShowConfirmInputDialogAsync<TView>(
        Func<TView, Task> confirmTask,
        Func<TView, Task> cancelTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase;
}