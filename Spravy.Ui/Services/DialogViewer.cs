using System;
using System.Threading.Tasks;
using Avalonia;
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

    public Task ShowContentDialogAsync<TView>(Action<TView>? setupView = null)
    {
        var content = Resolver.Get<TView>();

        if (content is null)
        {
            throw new NullReferenceException();
        }

        setupView?.Invoke(content);

        return ShowView(content, ContentDialogHostIdentifier);
    }

    public Task ShowProgressDialogAsync<TView>(Action<TView>? setupView = null)
    {
        var content = Resolver.Get<TView>();

        if (content is null)
        {
            throw new NullReferenceException();
        }

        setupView?.Invoke(content);

        return ShowView(content, ProgressDialogHostIdentifier);
    }

    public Task ShowErrorDialogAsync<TView>(Action<TView>? setupView = null)
    {
        var content = Resolver.Get<TView>();

        if (content is null)
        {
            throw new NullReferenceException();
        }

        setupView?.Invoke(content);

        return ShowView(content, ErrorDialogHostIdentifier);
    }

    public Task ShowInfoErrorDialogAsync<TView>(Func<TView, Task> okTask, Action<TView>? setupView = null)
    {
        var content = Resolver.Get<TView>();
        setupView?.Invoke(content);
        var infoViewModel = Resolver.Get<InfoViewModel>();
        infoViewModel.Content = content;
        infoViewModel.OkTask = view => okTask.Invoke((TView)view);

        return ShowView(infoViewModel, ErrorDialogHostIdentifier);
    }

    public Task ShowInfoInputDialogAsync<TView>(Func<TView, Task> okTask, Action<TView>? setupView = null)
    {
        var content = Resolver.Get<TView>();
        setupView?.Invoke(content);
        var infoViewModel = Resolver.Get<InfoViewModel>();
        infoViewModel.Content = content;
        infoViewModel.OkTask = view => okTask.Invoke((TView)view);

        return ShowView(infoViewModel, InputDialogHostIdentifier);
    }

    public Task ShowInputDialogAsync<TView>(Action<TView>? setupView = null)
    {
        var content = Resolver.Get<TView>();

        if (content is null)
        {
            throw new NullReferenceException();
        }

        setupView?.Invoke(content);

        return ShowView(content, InputDialogHostIdentifier);
    }

    public Task CloseProgressDialogAsync()
    {
        return SafeClose(ProgressDialogHostIdentifier);
    }

    public Task ShowConfirmContentDialogAsync<TView>(
        Func<TView, Task> confirmTask,
        Func<TView, Task> cancelTask,
        Action<TView>? setupView = null
    )
    {
        var content = Resolver.Get<TView>();
        setupView?.Invoke(content);
        var confirmViewModel = Resolver.Get<ConfirmViewModel>();
        confirmViewModel.Content = content;
        confirmViewModel.ConfirmTask = view => confirmTask.Invoke((TView)view);
        confirmViewModel.CancelTask = view => cancelTask.Invoke((TView)view);

        return ShowView(confirmViewModel, ContentDialogHostIdentifier);
    }

    public Task ShowConfirmInputDialogAsync<TView>(
        Func<TView, Task> confirmTask,
        Func<TView, Task> cancelTask,
        Action<TView>? setupView = null
    )
    {
        var content = Resolver.Get<TView>();
        setupView?.Invoke(content);
        var confirmViewModel = Resolver.Get<ConfirmViewModel>();
        confirmViewModel.Content = content;
        confirmViewModel.ConfirmTask = view => confirmTask.Invoke((TView)view);
        confirmViewModel.CancelTask = view => cancelTask.Invoke((TView)view);

        return ShowView(confirmViewModel, InputDialogHostIdentifier);
    }

    public Task CloseContentDialogAsync()
    {
        return SafeClose(ContentDialogHostIdentifier);
    }

    public Task CloseErrorDialogAsync()
    {
        return SafeClose(ErrorDialogHostIdentifier);
    }

    public Task CloseInputDialogAsync()
    {
        return SafeClose(InputDialogHostIdentifier);
    }

    private Task ShowView(object content, string identifier)
    {
        if (content is not StyledElement styledElement)
        {
            return DialogHost.Show(content, identifier);
        }

        if (styledElement.DataContext is not ViewModelBase viewModel)
        {
            return DialogHost.Show(content, identifier);
        }

        viewModel.ReleaseCommands();

        return DialogHost.Show(content, identifier);
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