using System;
using System.Threading.Tasks;
using DialogHostAvalonia;
using Spravy.Domain.Attributes;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Views;

namespace Spravy.Ui.Services;

public class DialogViewer : IDialogViewer
{
    public const string DefaultDialogIdentifier = "MainDialogHost";

    private readonly string dialogIdentifier;

    private static object? currentContent;

    public DialogViewer(string dialogIdentifier)
    {
        this.dialogIdentifier = dialogIdentifier;
    }

    [Inject]
    public required IResolver Resolver { get; init; }

    public Task<object?> ShowDialogAsync(Type contentType)
    {
        var content = Resolver.Resolve(contentType);

        return DialogHost.Show(content, dialogIdentifier);
    }

    Task<object?> IDialogViewer.ShowDialogAsync<TView>(Action<TView>? setup)
    {
        return ShowDialogAsync(setup);
    }

    public Task<object?> ShowDialogAsync<TView>(Action<TView>? setup) where TView : notnull
    {
        var content = Resolver.Resolve<TView>();
        setup?.Invoke(content);
        currentContent = content;

        return DialogHost.Show(content, dialogIdentifier);
    }

    public Task<object?> ShowConfirmDialogAsync<TView>(
        Func<TView, Task> cancelTask,
        Func<TView, Task> confirmTask,
        Action<TView>? setup = null
    ) where TView : notnull
    {
        var content = Resolver.Resolve<TView>();
        setup?.Invoke(content);
        var confirmView = Resolver.Resolve<ConfirmView>();
        var viewModel = confirmView.ViewModel.ThrowIfNull();
        viewModel.Content = content;
        viewModel.ConfirmTask = view => confirmTask.Invoke((TView)view);
        viewModel.CancelTask = view => cancelTask.Invoke((TView)view);
        currentContent = confirmView;

        return DialogHost.Show(confirmView, dialogIdentifier);
    }

    public void CloseDialog()
    {
        if (DialogHost.IsDialogOpen(dialogIdentifier))
        {
            DialogHost.Close(dialogIdentifier, currentContent);
        }
    }
}