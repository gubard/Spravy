using System;
using System.Threading;
using System.Threading.Tasks;
using Ninject;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.ViewModels;
using DialogHostAvalonia;
using Spravy.Domain.Extensions;

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

        await this.InvokeUIBackgroundAsync(() => setupView.Invoke(content));
        cancellationToken.ThrowIfCancellationRequested();
        await ShowView(content, ContentDialogHostIdentifier).ConfigureAwait(false);
    }

    public async Task ShowProgressDialogAsync<TView>(Action<TView> setupView, CancellationToken cancellationToken)
        where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();

        if (content is null)
        {
            throw new NullReferenceException();
        }

        await this.InvokeUIBackgroundAsync(() => setupView.Invoke(content));
        cancellationToken.ThrowIfCancellationRequested();
        await ShowView(content, ProgressDialogHostIdentifier).ConfigureAwait(false);
    }

    public async Task ShowErrorDialogAsync<TView>(Action<TView> setupView, CancellationToken cancellationToken)
        where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();

        if (content is null)
        {
            throw new NullReferenceException();
        }

        await this.InvokeUIBackgroundAsync(() => setupView.Invoke(content));
        cancellationToken.ThrowIfCancellationRequested();
        await ShowView(content, ErrorDialogHostIdentifier).ConfigureAwait(false);
    }

    public async Task ShowInfoErrorDialogAsync<TView>(
        Func<TView, Task> okTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    )
        where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();
        await this.InvokeUIBackgroundAsync(() => setupView.Invoke(content));
        var infoViewModel = Resolver.Get<InfoViewModel>();
        infoViewModel.Content = content;
        infoViewModel.OkTask = view => okTask.Invoke((TView)view);
        cancellationToken.ThrowIfCancellationRequested();
        await ShowView(infoViewModel, ErrorDialogHostIdentifier).ConfigureAwait(false);
    }

    public async Task ShowInfoInputDialogAsync<TView>(
        Func<TView, Task> okTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    )
        where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();
        await this.InvokeUIBackgroundAsync(() => setupView.Invoke(content));
        var infoViewModel = Resolver.Get<InfoViewModel>();
        infoViewModel.Content = content;
        infoViewModel.OkTask = view => okTask.Invoke((TView)view);
        cancellationToken.ThrowIfCancellationRequested();
        await ShowView(infoViewModel, InputDialogHostIdentifier).ConfigureAwait(false);
    }

    public async Task ShowInfoContentDialogAsync<TView>(
        Func<TView, Task> okTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();
        await this.InvokeUIBackgroundAsync(() => setupView.Invoke(content));
        var infoViewModel = Resolver.Get<InfoViewModel>();
        infoViewModel.Content = content;
        infoViewModel.OkTask = view => okTask.Invoke((TView)view);
        cancellationToken.ThrowIfCancellationRequested();
        await ShowView(infoViewModel, ContentDialogHostIdentifier).ConfigureAwait(false);
    }

    public async Task ShowInputDialogAsync<TView>(Action<TView> setupView, CancellationToken cancellationToken)
        where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();

        if (content is null)
        {
            throw new NullReferenceException();
        }

        await this.InvokeUIBackgroundAsync(() => setupView.Invoke(content));
        cancellationToken.ThrowIfCancellationRequested();
        await ShowView(content, InputDialogHostIdentifier).ConfigureAwait(false);
    }

    public Task CloseProgressDialogAsync(CancellationToken cancellationToken)
    {
        return SafeClose(ProgressDialogHostIdentifier);
    }

    public async Task<bool> CloseLastDialogAsync(CancellationToken cancellationToken)
    {
        if (DialogHost.IsDialogOpen(ProgressDialogHostIdentifier))
        {
            await SafeClose(ProgressDialogHostIdentifier);

            return true;
        }

        if (DialogHost.IsDialogOpen(ErrorDialogHostIdentifier))
        {
            await SafeClose(ErrorDialogHostIdentifier);

            return true;
        }

        if (DialogHost.IsDialogOpen(InputDialogHostIdentifier))
        {
            await SafeClose(InputDialogHostIdentifier);

            return true;
        }

        if (DialogHost.IsDialogOpen(ContentDialogHostIdentifier))
        {
            await SafeClose(ContentDialogHostIdentifier);

            return true;
        }

        return false;
    }

    public async Task ShowConfirmContentDialogAsync<TView>(
        Func<TView, Task> confirmTask,
        Func<TView, Task> cancelTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();
        await this.InvokeUIBackgroundAsync(() => setupView.Invoke(content));
        var confirmViewModel = Resolver.Get<ConfirmViewModel>();
        confirmViewModel.Content = content;
        confirmViewModel.ConfirmTask = view => confirmTask.Invoke((TView)view);
        confirmViewModel.CancelTask = view => cancelTask.Invoke((TView)view);
        cancellationToken.ThrowIfCancellationRequested();
        await ShowView(confirmViewModel, ContentDialogHostIdentifier).ConfigureAwait(false);
    }

    public async Task ShowConfirmInputDialogAsync<TView>(
        Func<TView, Task> confirmTask,
        Func<TView, Task> cancelTask,
        Action<TView> setupView,
        CancellationToken cancellationToken
    ) where TView : ViewModelBase
    {
        var content = Resolver.Get<TView>();
        await this.InvokeUIBackgroundAsync(() => setupView.Invoke(content));
        var confirmViewModel = Resolver.Get<ConfirmViewModel>();
        confirmViewModel.Content = content;
        confirmViewModel.ConfirmTask = view => confirmTask.Invoke((TView)view);
        confirmViewModel.CancelTask = view => cancelTask.Invoke((TView)view);
        cancellationToken.ThrowIfCancellationRequested();
        await ShowView(confirmViewModel, InputDialogHostIdentifier).ConfigureAwait(false);
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

        return this.InvokeUIAsync(
            () => DialogHost.Show(
                content,
                identifier
            )
        );
    }

    private async Task SafeClose(string identifier)
    {
        if (!DialogHost.IsDialogOpen(identifier))
        {
            return;
        }

        var content = DialogHost.GetDialogSession(identifier).ThrowIfNull().Content.ThrowIfNull();

        if (content is ISaveState saveState)
        {
            await saveState.SaveStateAsync();
        }

        await this.InvokeUIAsync(() => DialogHost.Close(identifier));
    }
}