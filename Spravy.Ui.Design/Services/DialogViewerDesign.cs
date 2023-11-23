using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.Design.Services;

public class DialogViewerDesign : IDialogViewer
{
    public Task ShowContentDialogAsync<TView>(Action<TView>? setupView = null) where TView : ViewModelBase
    {
        return Task.CompletedTask;
    }

    public Task ShowProgressDialogAsync<TView>(Action<TView>? setupView = null) where TView : ViewModelBase
    {
        return Task.CompletedTask;
    }

    public Task ShowErrorDialogAsync<TView>(Action<TView>? setupView = null) where TView : ViewModelBase
    {
        return Task.CompletedTask;
    }

    public Task ShowInfoErrorDialogAsync<TView>(Func<TView, Task> okTask, Action<TView>? setupView = null)
        where TView : ViewModelBase
    {
        return Task.CompletedTask;
    }

    public Task ShowInfoInputDialogAsync<TView>(Func<TView, Task> okTask, Action<TView>? setupView = null)
        where TView : ViewModelBase
    {
        return Task.CompletedTask;
    }

    public Task ShowInputDialogAsync<TView>(Action<TView>? setupView = null) where TView : ViewModelBase
    {
        return Task.CompletedTask;
    }

    public Task CloseContentDialogAsync()
    {
        return Task.CompletedTask;
    }

    public Task CloseErrorDialogAsync()
    {
        return Task.CompletedTask;
    }

    public Task CloseInputDialogAsync()
    {
        return Task.CompletedTask;
    }

    public Task CloseProgressDialogAsync()
    {
        return Task.CompletedTask;
    }

    public Task ShowConfirmContentDialogAsync<TView>(
        Func<TView, Task> confirmTask,
        Func<TView, Task> cancelTask,
        Action<TView>? setupView = null
    ) where TView : ViewModelBase
    {
        return Task.CompletedTask;
    }

    public Task ShowConfirmInputDialogAsync<TView>(
        Func<TView, Task> confirmTask,
        Func<TView, Task> cancelTask,
        Action<TView>? setupView = null
    ) where TView : ViewModelBase
    {
        return Task.CompletedTask;
    }

    public Task ShowContentDialogAsync<TView>(Action<TView> setupView, CancellationToken cancellationToken) where TView : ViewModelBase
    {
        throw new NotImplementedException();
    }

    public Task ShowProgressDialogAsync<TView>(Action<TView> setupView, CancellationToken cancellationToken) where TView : ViewModelBase
    {
        throw new NotImplementedException();
    }

    public Task ShowErrorDialogAsync<TView>(Action<TView> setupView, CancellationToken cancellationToken) where TView : ViewModelBase
    {
        throw new NotImplementedException();
    }

    public Task ShowInfoErrorDialogAsync<TView>(Func<TView, Task> okTask, Action<TView> setupView, CancellationToken cancellationToken) where TView : ViewModelBase
    {
        throw new NotImplementedException();
    }

    public Task ShowInfoInputDialogAsync<TView>(Func<TView, Task> okTask, Action<TView> setupView, CancellationToken cancellationToken) where TView : ViewModelBase
    {
        throw new NotImplementedException();
    }

    public Task ShowInfoContentDialogAsync<TView>(Func<TView, Task> okTask, Action<TView> setupView, CancellationToken cancellationToken) where TView : ViewModelBase
    {
        throw new NotImplementedException();
    }

    public Task ShowInputDialogAsync<TView>(Action<TView> setupView, CancellationToken cancellationToken) where TView : ViewModelBase
    {
        throw new NotImplementedException();
    }

    public Task CloseContentDialogAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task CloseErrorDialogAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task CloseInputDialogAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task CloseProgressDialogAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task ShowConfirmContentDialogAsync<TView>(
        Func<TView, Task> confirmTask,
        Func<TView, Task> cancelTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        throw new NotImplementedException();
    }

    public Task ShowConfirmInputDialogAsync<TView>(
        Func<TView, Task> confirmTask,
        Func<TView, Task> cancelTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        throw new NotImplementedException();
    }
}