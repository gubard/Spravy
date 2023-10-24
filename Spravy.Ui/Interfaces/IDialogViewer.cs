using System;
using System.Threading.Tasks;

namespace Spravy.Ui.Interfaces;

public interface IDialogViewer
{
    Task ShowContentDialogAsync<TView>(Action<TView>? setupView = null);
    Task ShowProgressDialogAsync<TView>(Action<TView>? setupView = null);
    Task ShowErrorDialogAsync<TView>(Action<TView>? setupView = null);
    Task ShowInfoErrorDialogAsync<TView>(Func<TView, Task> okTask, Action<TView>? setupView = null);
    Task ShowInfoInputDialogAsync<TView>(Func<TView, Task> okTask, Action<TView>? setupView = null);
    Task ShowInputDialogAsync<TView>(Action<TView>? setupView = null);
    Task CloseContentDialogAsync();
    Task CloseErrorDialogAsync();
    Task CloseInputDialogAsync();
    Task CloseProgressDialogAsync();

    Task ShowConfirmContentDialogAsync<TView>(
        Func<TView, Task> confirmTask,
        Func<TView, Task> cancelTask,
        Action<TView>? setupView = null
    );

    Task ShowConfirmInputDialogAsync<TView>(
        Func<TView, Task> confirmTask,
        Func<TView, Task> cancelTask,
        Action<TView>? setupView = null
    );
}