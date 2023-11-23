using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using DialogHostAvalonia;
using Ninject;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Services;

public class DialogViewer : IDialogViewer
{
    public const string ContentDialogHostIdentifier = "ContentDialogHost";
    public const string ErrorDialogHostIdentifier = "ErrorDialogHost";
    public const string InputDialogHostIdentifier = "InputDialogHost";
    public const string ProgressDialogHostIdentifier = "ProgressDialogHost";

    [Inject]
    public required IKernel Resolver { get; init; }

    public async Task ShowContentDialogAsync<TView>(Action<TView> setupView, CancellationToken cancellationToken)
        where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();

        if (content is null)
        {
            throw new NullReferenceException();
        }

        await Dispatcher.UIThread.InvokeAsync(() => setupView.Invoke(content));
        cancellationToken.ThrowIfCancellationRequested();
        await ShowView(content, ContentDialogHostIdentifier);
    }

    public async Task ShowProgressDialogAsync<TView>(Action<TView> setupView, CancellationToken cancellationToken)
        where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();

        if (content is null)
        {
            throw new NullReferenceException();
        }

        await Dispatcher.UIThread.InvokeAsync(() => setupView.Invoke(content));
        cancellationToken.ThrowIfCancellationRequested();
        await ShowView(content, ProgressDialogHostIdentifier);
    }

    public async Task ShowErrorDialogAsync<TView>(Action<TView> setupView, CancellationToken cancellationToken)
        where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();

        if (content is null)
        {
            throw new NullReferenceException();
        }

        await Dispatcher.UIThread.InvokeAsync(() => setupView.Invoke(content));
        cancellationToken.ThrowIfCancellationRequested();
        await ShowView(content, ErrorDialogHostIdentifier);
    }

    public async Task ShowInfoErrorDialogAsync<TView>(
        Func<TView, Task> okTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    )
        where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();
        await Dispatcher.UIThread.InvokeAsync(() => setupView.Invoke(content));
        var infoViewModel = Resolver.Get<InfoViewModel>();
        infoViewModel.Content = content;
        infoViewModel.OkTask = view => okTask.Invoke((TView)view);
        cancellationToken.ThrowIfCancellationRequested();
        await ShowView(infoViewModel, ErrorDialogHostIdentifier);
    }

    public async Task ShowInfoInputDialogAsync<TView>(
        Func<TView, Task> okTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    )
        where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();
        await Dispatcher.UIThread.InvokeAsync(() => setupView.Invoke(content));
        var infoViewModel = Resolver.Get<InfoViewModel>();
        infoViewModel.Content = content;
        infoViewModel.OkTask = view => okTask.Invoke((TView)view);
        cancellationToken.ThrowIfCancellationRequested();
        await ShowView(infoViewModel, InputDialogHostIdentifier);
    }

    public async Task ShowInfoContentDialogAsync<TView>(
        Func<TView, Task> okTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();
        await Dispatcher.UIThread.InvokeAsync(() => setupView.Invoke(content));
        var infoViewModel = Resolver.Get<InfoViewModel>();
        infoViewModel.Content = content;
        infoViewModel.OkTask = view => okTask.Invoke((TView)view);
        cancellationToken.ThrowIfCancellationRequested();
        await ShowView(infoViewModel, ContentDialogHostIdentifier);
    }

    public async Task ShowInputDialogAsync<TView>(Action<TView> setupView, CancellationToken cancellationToken)
        where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();

        if (content is null)
        {
            throw new NullReferenceException();
        }

        await Dispatcher.UIThread.InvokeAsync(() => setupView.Invoke(content));
        cancellationToken.ThrowIfCancellationRequested();
        await ShowView(content, InputDialogHostIdentifier);
    }

    public Task CloseProgressDialogAsync(CancellationToken cancellationToken)
    {
        return SafeClose(ProgressDialogHostIdentifier);
    }

    public async Task ShowConfirmContentDialogAsync<TView>(
        Func<TView, Task> confirmTask,
        Func<TView, Task> cancelTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();
        await Dispatcher.UIThread.InvokeAsync(() => setupView.Invoke(content));
        var confirmViewModel = Resolver.Get<ConfirmViewModel>();
        confirmViewModel.Content = content;
        confirmViewModel.ConfirmTask = view => confirmTask.Invoke((TView)view);
        confirmViewModel.CancelTask = view => cancelTask.Invoke((TView)view);
        cancellationToken.ThrowIfCancellationRequested();
        await ShowView(confirmViewModel, ContentDialogHostIdentifier);
    }

    public async Task ShowConfirmInputDialogAsync<TView>(
        Func<TView, Task> confirmTask,
        Func<TView, Task> cancelTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();
        await Dispatcher.UIThread.InvokeAsync(() => setupView.Invoke(content));
        var confirmViewModel = Resolver.Get<ConfirmViewModel>();
        confirmViewModel.Content = content;
        confirmViewModel.ConfirmTask = view => confirmTask.Invoke((TView)view);
        confirmViewModel.CancelTask = view => cancelTask.Invoke((TView)view);
        cancellationToken.ThrowIfCancellationRequested();
        await ShowView(confirmViewModel, InputDialogHostIdentifier);
    }

    public Task CloseContentDialogAsync(CancellationToken cancellationToken)
    {
        return SafeClose(ContentDialogHostIdentifier);
    }

    public Task CloseErrorDialogAsync(CancellationToken cancellationToken)
    {
        return SafeClose(ErrorDialogHostIdentifier);
    }

    public Task CloseInputDialogAsync(CancellationToken cancellationToken)
    {
        return SafeClose(InputDialogHostIdentifier);
    }

    private Task ShowView(object content, string identifier)
    {
        if (DialogHost.IsDialogOpen(identifier))
        {
            return Task.CompletedTask;
        }

        return Dispatcher.UIThread.InvokeAsync(() => DialogHost.Show(content, identifier));
    }

    private async Task SafeClose(string identifier)
    {
        if (!DialogHost.IsDialogOpen(identifier))
        {
            return;
        }

        await Dispatcher.UIThread.InvokeAsync(() => DialogHost.Close(identifier));
    }
}