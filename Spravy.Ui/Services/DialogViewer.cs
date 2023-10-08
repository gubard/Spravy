using System;
using System.Threading.Tasks;
using DialogHostAvalonia;
using Ninject;
using Spravy.Domain.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Views;

namespace Spravy.Ui.Services;

public class DialogViewer : IDialogViewer
{
    public const string ContentDialogHostIdentifier = "ContentDialogHost";
    public const string ErrorDialogHostIdentifier = "ErrorDialogHost";
    public const string InputDialogHostIdentifier = "InputDialogHost";
    public const string ProgressDialogHostIdentifier = "ProgressDialogHost";

    [Inject]
    public required IKernel Resolver { get; init; }

    public Task ShowContentDialogAsync<TView>(Action<TView>? setupView = null)
    {
        var content = Resolver.Get<TView>().ThrowIfNull();
        setupView?.Invoke(content);

        return DialogHost.Show(content, ContentDialogHostIdentifier);
    }

    public Task ShowProgressDialogAsync<TView>(Action<TView>? setupView = null)
    {
        var content = Resolver.Get<TView>().ThrowIfNull();
        setupView?.Invoke(content);

        return DialogHost.Show(content, ProgressDialogHostIdentifier);
    }

    public Task ShowErrorDialogAsync<TView>(Action<TView>? setupView = null)
    {
        var content = Resolver.Get<TView>().ThrowIfNull();
        setupView?.Invoke(content);

        return DialogHost.Show(content, ErrorDialogHostIdentifier);
    }

    public Task ShowInfoErrorDialogAsync<TView>(Func<TView, Task> okTask, Action<TView>? setupView = null)
    {
        var content = Resolver.Get<TView>();
        setupView?.Invoke(content);
        var confirmView = Resolver.Get<InfoView>();
        var viewModel = confirmView.ViewModel.ThrowIfNull();
        viewModel.Content = content;
        viewModel.OkTask = view => okTask.Invoke((TView)view);

        return DialogHost.Show(confirmView, ErrorDialogHostIdentifier);
    }

    public Task ShowInfoInputDialogAsync<TView>(Func<TView, Task> okTask, Action<TView>? setupView = null)
    {
        var content = Resolver.Get<TView>();
        setupView?.Invoke(content);
        var confirmView = Resolver.Get<InfoView>();
        var viewModel = confirmView.ViewModel.ThrowIfNull();
        viewModel.Content = content;
        viewModel.OkTask = view => okTask.Invoke((TView)view);

        return DialogHost.Show(confirmView, InputDialogHostIdentifier);
    }

    public Task ShowInputDialogAsync<TView>(Action<TView>? setupView = null)
    {
        var content = Resolver.Get<TView>().ThrowIfNull();
        setupView?.Invoke(content);

        return DialogHost.Show(content, InputDialogHostIdentifier);
    }

    public Task CloseProgressDialogAsync()
    {
        SafeClose(ProgressDialogHostIdentifier);

        return Task.CompletedTask;
    }

    public Task ShowConfirmContentDialogAsync<TView>(
        Func<TView, Task> confirmTask,
        Func<TView, Task> cancelTask,
        Action<TView>? setupView = null
    )
    {
        var content = Resolver.Get<TView>();
        setupView?.Invoke(content);
        var confirmView = Resolver.Get<ConfirmView>();
        var viewModel = confirmView.ViewModel.ThrowIfNull();
        viewModel.Content = content;
        viewModel.ConfirmTask = view => confirmTask.Invoke((TView)view);
        viewModel.CancelTask = view => cancelTask.Invoke((TView)view);

        return DialogHost.Show(confirmView, ContentDialogHostIdentifier);
    }

    public Task ShowConfirmInputDialogAsync<TView>(
        Func<TView, Task> confirmTask,
        Func<TView, Task> cancelTask,
        Action<TView>? setupView = null
    )
    {
        var content = Resolver.Get<TView>();
        setupView?.Invoke(content);
        var confirmView = Resolver.Get<ConfirmView>();
        var viewModel = confirmView.ViewModel.ThrowIfNull();
        viewModel.Content = content;
        viewModel.ConfirmTask = view => confirmTask.Invoke((TView)view);
        viewModel.CancelTask = view => cancelTask.Invoke((TView)view);

        return DialogHost.Show(confirmView, InputDialogHostIdentifier);
    }

    public Task CloseContentDialogAsync()
    {
        SafeClose(ContentDialogHostIdentifier);

        return Task.CompletedTask;
    }

    public Task CloseErrorDialogAsync()
    {
        SafeClose(ErrorDialogHostIdentifier);

        return Task.CompletedTask;
    }

    public Task CloseInputDialogAsync()
    {
        SafeClose(InputDialogHostIdentifier);

        return Task.CompletedTask;
    }

    private void SafeClose(string identifier)
    {
        if (!DialogHost.IsDialogOpen(identifier))
        {
            return;
        }

        DialogHost.Close(identifier);
    }
}