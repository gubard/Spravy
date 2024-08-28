namespace Spravy.Ui.Extensions;

public static class DialogViewerExtension
{
    public static Cvtar ShowConfirmDialogAsync<TViewModel>(
        this IDialogViewer dialogViewer,
        IViewFactory viewFactory,
        DialogViewLayer layer,
        TViewModel viewModel,
        Func<TViewModel, Cvtar> confirmTask,
        CancellationToken ct
    )
        where TViewModel : IDialogable
    {
        var confirm = viewFactory.CreateConfirmViewModel(
            viewModel,
            obj => obj.CastObject<TViewModel>(nameof(obj)).IfSuccessAsync(confirmTask.Invoke, ct),
            _ => dialogViewer.CloseDialogAsync(layer, ct)
        );

        return dialogViewer.ShowDialogAsync(layer, confirm, ct);
    }

    public static Cvtar ShowInfoDialogAsync<TViewModel>(
        this IDialogViewer dialogViewer,
        IViewFactory viewFactory,
        DialogViewLayer layer,
        TViewModel viewModel,
        CancellationToken ct
    )
        where TViewModel : IDialogable
    {
        var info = viewFactory.CreateInfoViewModel(
            viewModel,
            _ => dialogViewer.CloseDialogAsync(layer, ct)
        );

        return dialogViewer.ShowDialogAsync(layer, info, ct);
    }
}
